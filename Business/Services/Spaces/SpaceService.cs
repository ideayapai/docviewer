using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Infrasturcture.DomainObjects;
using Infrasturcture.Errors;
using Messages;
using Repository;
using Services.CacheService;
using Services.Contracts;
using Services.Enums;
using Services.Messages;

namespace Services.Spaces
{
    /// <summary>
    /// 空间文件夹
    /// </summary>
    public class SpaceService
    {
        private readonly IBaseRepository<Space> _spaceRepository;
        private readonly MessageBus _bus;
        private readonly SpaceCacheService _cacheService;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();


        public SpaceService(IBaseRepository<Space> spaceRepository,
                            SpaceCacheService cacheService,
                            MessageBus bus)
        {
            _spaceRepository = spaceRepository;
            _cacheService = cacheService;
            _bus = bus;
        }


        public SpaceObject Add(string parentId, string spaceSeqNo, string name, string userId, string userName, string depId, Visible visible)
        {
            var spaceObject = MakeSpaceContract(parentId, spaceSeqNo, name, userId, userName, depId, (int)visible);
            return Add(spaceObject);
        }

        /// <summary>
        /// 001_西山区|4567_虹桥立交|8889_日常巡检 解析为三个文件夹
        /// 西山区(001)
        ///   |
        ///   |---虹桥立交(4567)
        ///         |
        ///         |----日常巡检(8889)
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="path"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public SpaceObject MakeSpace(string parentId, string path, string userId, string userName, string depId, Visible visible)
        {
            _logger.Debug("Enter MakeSpace");

            SpaceObject space = GetDefaultSpace();
            if (string.IsNullOrWhiteSpace(parentId))
            {
                parentId = space.Id.ToString();
            }

            string[] paths = path.Split('|');
            foreach (var parameter in paths)
            {
                string spaceSeqNo = string.Empty;
                string spaceName = string.Empty;
                if (TryParse(parameter, ref spaceSeqNo, ref spaceName))
                {
                    space = GetSpaceBySeqNo(spaceSeqNo);
                    if (space == null)
                    {
                        space = Add(parentId, spaceSeqNo, spaceName, userId, userName,  depId, visible);
                    }
                    if (space.IsDelete)
                    {
                        space = Recovery(space.Id.ToString());
                    }
                    parentId = space.Id.ToString();
                }
            }

            return space;
        }

        

        /// <summary>
        /// 添加文件夹
        /// </summary>
        /// <param name="space"></param>
        /// <returns></returns>
        public SpaceObject Add(SpaceObject space)
        {
            _logger.Info("Add");

            space = _spaceRepository.Add(space.ToEntity<Space>()).ToObject<SpaceObject>();
            if (space != null)
            {
                _bus.Send(new CreateSpaceMessage { Content = space });
                _cacheService.Add(space);
            }
          
            return space;
        }

        /// <summary>
        /// 修改文件夹
        /// </summary>
        /// <param name="space"></param>
        /// <returns></returns>
        public SpaceObject Update(SpaceObject space)
        {
            _logger.Info("Update");

            space = _spaceRepository.Update(f => f.Id == space.Id, space.ToEntity<Space>()).ToObject<SpaceObject>();
            if (space != null)
            {
                _cacheService.Update(space);
                _bus.Send(new UpdateSpaceMessage { Content = space });
            }
           
            return space;
        }

        /// <summary>
        /// 移动文件夹
        /// </summary>
        /// <param name="id"></param>
        /// <param name="spaceid"></param>
        /// <returns></returns>
        public SpaceObject Move(string id, string spaceid)
        {
            _logger.InfoFormat("Move id{0}, spaceid:{1}", id, spaceid);
            var space = GetSpace(id);
            if (space != null)
            {
                space.ParentId = spaceid;
                return Update(space);
            }

            throw new Exception(ErrorMessages.GetErrorMessages(ErrorMessages.SpaceNotExist));
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SpaceObject ReName(string id, string name)
        {
            _logger.InfoFormat("ReName id:{0}, name:{1}", id, name);

            var space = GetSpace(id);
            if (space != null)
            {
                space.SpaceName = name;
                
                return Update(space);
            }

            throw new Exception(ErrorMessages.GetErrorMessages(ErrorMessages.SpaceNotExist));
            
        }

        /// <summary>
        /// 本文件夹和所有子文件夹
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SpaceObject MoveToTrash(string Id)
        {
            _logger.InfoFormat("MoveToTrash {0}", Id);

            return RecoveryOrTrash(Id, true);
        }

        /// <summary>
        /// 批量移动本文件夹和所有子文件夹到回收站
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SpaceObject> MoveToTrash(string[] ids)
        {
            _logger.Info("SpaceService 批量移动到回收站:");

            return ids.Select(MoveToTrash).ToList();
        }

        /// <summary>
        /// 设置权限,TODO:添加单元测试
        /// </summary>
        /// <param name="id"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        //   public enum Visible
        //   {
        //      Public,0
        //      Dep,1
        //      Private,2
        //   }
        public SpaceObject SetVisiblity(string Id, Visible visible)
        {
            _logger.Info("SpaceService 设置权限");

            var space = _spaceRepository.Update(f => f.Id.ToString() == Id, f => f.Visible = (int)visible).ToObject<SpaceObject>();
            
            if (space != null)
            {
                _cacheService.Update(space);

                SetChildrenVisiblity(Id, visible);
            }


            return space;
        }

        private void SetChildrenVisiblity(string parentId, Visible visible)
        {
            var spaces = _spaceRepository.UpdateAll(f => f.ParentId == parentId, f => f.Visible = (int)visible).ConvertAll(
                f => f.ToObject<SpaceObject>());

            if (spaces.Count > 0)
            {
                _cacheService.UpdateAll(spaces);

                foreach (var entity in spaces)
                {
                    SetChildrenVisiblity(entity.Id.ToString(), visible);
                }
            }
        }

        /// <summary>
        /// 本文件夹和所有子文件夹全部还原
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SpaceObject Recovery(string Id)
        {
            _logger.InfoFormat("SpaceService 恢复 {0}", Id);

            return RecoveryOrTrash(Id, false);
        }

        /// <summary>
        /// 批量还原文件夹和子文件夹
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SpaceObject> Recovery(string[] ids)
        {
            _logger.Info("SpaceService 批量恢复:");

            return ids.Select(Recovery).ToList();
        }

        /// <summary>
        /// 批量删除或还原文件夹和子文件夹
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="isdelete"></param>
        /// <returns></returns>
        private SpaceObject RecoveryOrTrash(string Id, bool isdelete)
        {
            var space = _spaceRepository.Update(f => f.Id.ToString() == Id, f => f.IsDelete = isdelete).ToObject<SpaceObject>();
            if (space != null)
            {
                _cacheService.Update(space);

                if (isdelete)
                {
                    _bus.Send(new TrashSpaceMessage { Content = space });
                }
                else
                {
                    _bus.Send(new RecoverySpaceMessage { Content = space });
                }

                RecoveryOrTrashChild(Id, isdelete);
            }
            

            return space;
        }

        private void RecoveryOrTrashChild(string parentId, bool isdelete)
        {
            var spaces = _spaceRepository.UpdateAll(f => f.ParentId == parentId, f => f.IsDelete = isdelete).ConvertAll(
                f => f.ToObject<SpaceObject>());

            if (spaces.Count > 0)
            {
                _cacheService.UpdateAll(spaces);

                if (isdelete)
                {
                    _bus.Send(new RecoverSpacesMessage{ Contents = new List<SpaceObject>(spaces) });
                }
                else
                {
                    _bus.Send(new TrashSpacesMessage { Contents = new List<SpaceObject>(spaces) });
                }

                foreach (var entity in spaces)
                {
                    RecoveryOrTrashChild(entity.Id.ToString(), isdelete);
                }
            }
            
        }

        /// <summary>
        /// 本文件夹和所有子文件夹全部清空
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public void Delete(string Id)
        {
            //TODO:放到回收站的时候，已经删除文件夹，此时是否需要全部删除呢?

            var result = _spaceRepository.Delete(f => f.Id.ToString() == Id);
            
            if (result)
            {
                _cacheService.Delete(Id);

                var spaces = GetChildSpaces(Id);
                foreach (var entity in spaces)
                {
                    Delete(entity.Id.ToString());
                }
            }
        }

        /// <summary>
        /// 批量清空文件夹和子文件夹
        /// </summary>
        /// <param name="ids"></param>
        public void DeleteAll(string[] ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        /// <summary>
        /// 批量删除某用户的文件夹和子文件夹
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteAll(string userId)
        {
            _logger.Info("DeleteAll,userId:" + userId);

            var documents = GetTrashSpaces(userId);
            string[] ids = documents.Select(document => document.Id.ToString()).ToArray();
            DeleteAll(ids);
       }

      
        /// <summary>
        /// 获取子文件夹，包含回收站
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<SpaceObject> GetChildSpaces(string parentId)
        {
            _logger.Info("GetChildSpaces");

            if (string.IsNullOrWhiteSpace(parentId))
            {
                return new List<SpaceObject>();
            }

            return GetAllSpaces().FindAll(f => f.ParentId == parentId && f.Id.ToString() != parentId);
        }


        /// <summary>
        /// 获取所有未删除的文件夹
        /// </summary>
        /// <returns></returns>
        public List<SpaceObject> GetChildren()
        {
            _logger.Info("GetChildren");

            return GetAllSpaces().FindAll(f => !f.IsDelete);
        }

        /// <summary>
        /// 获取所有父亲结点
        /// </summary>
        /// <param name="spaceId"></param>
        /// <returns></returns>
        public List<SpaceObject> GetParentsChain(string spaceId)
        {
            var list = new List<SpaceObject>();

            do
            {
                var spaceObject = GetSpace(spaceId);
                if (spaceObject == null)
                {
                    break;
                }

                list.Add(spaceObject);
                if (spaceObject.ParentId == string.Empty)
                {
                    break;
                }
                spaceId = spaceObject.ParentId;

            } while (true);

            list.Reverse();

            return list;
        }

        public List<SpaceObject> GetVisibleSpaces(string userId)
        {
            _logger.Info("GetVisibleSpaces");

            return GetAllUndeleteSpaces().FindAll(VisiblePredicate(userId));
        }

        /// <summary>
        /// 获取当前用户和部门的所有文件夹(包含根目录)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<SpaceObject> GetVisibleSpaces(string userId, string depId)
        {
            _logger.Info("GetVisibleSpaces");

            return GetAllUndeleteSpaces().FindAll(VisiblePredicate(userId, depId));
        }

        private static Predicate<SpaceObject> VisiblePredicate(string userId, string depId=null)
        {
            if (depId != null)
            {
                return f => (f.CreateUserId == userId ||
                        (f.DepId == depId && f.Visible == (int)Visible.Dep) ||
                        f.Visible == (int)Visible.Public);
            }
            else
            {
                return f => (f.CreateUserId == userId ||
                          f.Visible == (int)Visible.Public);
                
            }
           
        }

        public List<SpaceObject> GetAllUndeleteSpaces()
        {
            return GetAllSpaces().FindAll(f => !f.IsDelete || f.IsDefault);
        }


        /// <summary>
        /// 获取未放到回收站的子文件夹
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public List<SpaceObject> GetChildren(string parentId, string userId, string depId)
        {
            _logger.InfoFormat("GetChildren parentId:{0}, userId:{1}", parentId, userId);

            if (string.IsNullOrWhiteSpace(parentId) || string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(depId))
            {
                return new List<SpaceObject>();
            }

            return GetChildren().FindAll(f => f.ParentId == parentId && f.Id.ToString() != parentId).FindAll(VisiblePredicate(userId, depId));

        }

        /// <summary>
        /// 获取回收站的子文件夹
        /// </summary>
        /// <returns>文件夹列表</returns>
        public List<SpaceObject> GetTrashSpaces(string userId)
        {
            _logger.Info("GetTrashSpaces");

            return GetAllTrashSpaces().FindAll(f=> f.CreateUserId == userId);
        }

        public List<SpaceObject> GetAllTrashSpaces()
        {
            _logger.Info("GetAllTrashSpaces");

            return GetAllSpaces().FindAll(f => f.IsDelete);
        }


        /// <summary>
        /// 获取所有文件夹
        /// </summary>
        /// <returns></returns>
        public List<SpaceObject> GetAllSpaces()
        {
            _logger.Info("Get All Spaces");

            return _cacheService.GetOrAdd(()=>_spaceRepository.GetAll(f => true).ConvertAll(f => f.ToObject<SpaceObject>())).OrderByDescending(f=>f.UpdateTime).ToList();
        }

        /// <summary>
        /// 获取当前空间信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public SpaceObject GetSpace(string Id)
        {
            _logger.InfoFormat("GetSpace:{0}", Id);

            if (string.IsNullOrWhiteSpace(Id) || Id=="null")
            {
                return GetDefaultSpace();
            }
            return GetSpace(Id, () => _spaceRepository.Get(f => f.Id == Guid.Parse(Id)).ToObject<SpaceObject>());
        }

        public SpaceObject GetSpaceBySeqNo(string spaceSeqNo)
        {
            _logger.InfoFormat("GetSpaceBySeqNo:{0}", spaceSeqNo);

            return GetSpace(spaceSeqNo, () => _spaceRepository.Get(f => f.SpaceSeqNo == spaceSeqNo).ToObject<SpaceObject>());
        }

       
        /// <summary>
        /// 获取当前空间信息重载
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="howToGet"></param>
        /// <returns></returns>
        public SpaceObject GetSpace(string Id, Func<SpaceObject> howToGet)
        {
            return _cacheService.GetOrAdd(Id, howToGet);
        }

        /// <summary>
        /// 获取某个用户的默认空间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SpaceObject GetDefaultSpace()
        {
            _logger.InfoFormat("GetDefaultSpace");

            return _spaceRepository.Get(f => f.IsDefault).ToObject<SpaceObject>();  
        }

        public SpaceObject GetOrAddDefaultSpace(string userName, string userId, string depId)
        {
            int count = _spaceRepository.Count(f => true);
            var space = _spaceRepository.Get(f => f.IsDefault);
            if (count == 0 || space == null)
            {
                var entity = new Space
                                 {
                                     Id = Guid.NewGuid(),
                                     ParentId = String.Empty,
                                     SpaceSeqNo = Guid.NewGuid().ToString(),
                                     CreateTime = DateTime.Now,
                                     CreateUserId = userId,
                                     CreateUserName = userName,
                                     FileCount = 0,
                                     SpaceName = "所有空间",
                                     SpaceSize = 0,
                                     UpdateTime = DateTime.Now,
                                     DepId = depId,
                                     Visible = (int)Visible.Public,
                                     IsDefault = true,
                                 };
                return Add(entity.ToObject<SpaceObject>());  
            }

            return _spaceRepository.Get(f => f.IsDefault).ToObject<SpaceObject>();    
        }

        private static SpaceObject MakeSpaceContract(string parentId, string spaceSeqNo, string name, string userId, string userName, string depId, int visible)
        {
            var spaceObject = new SpaceObject
            {
                Id = Guid.NewGuid(),
                SpaceSeqNo = spaceSeqNo,
                SpaceName = name,
                UpdateTime = DateTime.Now,
                CreateTime = DateTime.Now,
                ParentId = parentId,
                CreateUserId = userId,
                CreateUserName = userName,
                Visible = visible,
                DepId = depId,
            };
            return spaceObject;
        }

        public bool TryParse(string parameter, ref string spaceSeqNo, ref string spaceName)
        {
            string[] variables = parameter.Split('_');
            if (variables.Length != 2)
            {
                return false;
            }

            spaceSeqNo = variables[0];
            spaceName = variables[1];
            return true;
        }

       
    }
}
