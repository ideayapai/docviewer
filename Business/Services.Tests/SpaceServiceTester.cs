using System;
using Ninject;
using NUnit.Framework;
using Services.Contracts;
using Services.Enums;
using Services.Ioc;
using Services.Spaces;

namespace Services.Tests
{
    [TestFixture]
    public class SpaceServiceTester
    {
        private readonly SpaceService _spaceService = ServiceActivator.Kernel.Get<SpaceService>();

         
        [Test]
        public void should_get_all_spaces()
        {
            var spaces = _spaceService.GetAllSpaces();
            Assert.IsNotNull(spaces);
        }



        [Test]
        public void should_pass_rename_process()
        {
            int count = _spaceService.GetAllSpaces().Count;

            string userId = Guid.NewGuid().ToString();
            string depId = Guid.NewGuid().ToString();

            var space = MakeSimpleSpace(userId, depId);

            Assert.IsNotNull(_spaceService.Add(space));
            var result = _spaceService.GetSpace(space.Id.ToString());
            Assert.AreEqual(result.SpaceName, "技术空间");

            Assert.IsNotNull(_spaceService.ReName(space.Id.ToString(), "产品空间"));
            space = _spaceService.GetSpace(space.Id.ToString());
            Assert.AreEqual(space.SpaceName, @"产品空间");


            _spaceService.Delete(space.Id.ToString());
            result = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsNull(result);

            int count2 = _spaceService.GetAllSpaces().Count;
            Assert.AreEqual(count, count2);
        }

        [Test]
        public void should_pass_set_visilbity_spaces_process()
        {
            int count = _spaceService.GetAllSpaces().Count;
            string userId = Guid.NewGuid().ToString();
            string depId = Guid.NewGuid().ToString();

            var space = MakeSimpleSpace(userId, depId);
            var childspace = MakeChildSpace(space.Id.ToString(), userId,depId);
            var childspace2 = MakeChildSpace(childspace.Id.ToString(), userId,depId);
            var childspace_son = MakeChildSpace(childspace2.Id.ToString(), userId, depId);

            Assert.IsNotNull(_spaceService.Add(space));
            Assert.IsNotNull(_spaceService.Add(childspace));
            Assert.IsNotNull(_spaceService.Add(childspace2));
            Assert.IsNotNull(_spaceService.Add(childspace_son));

            //设置权限为私有
            space = TestVisiblity(space, Visible.Private, ref childspace, ref childspace2, ref childspace_son);

            //设置权限为部门可见
            space = TestVisiblity(space, Visible.Dep, ref childspace, ref childspace2, ref childspace_son);

            //设置权限为公开可见
            space = TestVisiblity(space, Visible.Public, ref childspace, ref childspace2, ref childspace_son);

            _spaceService.Delete(space.Id.ToString());
            var result = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsNull(result);
            result = _spaceService.GetSpace(childspace.Id.ToString());
            Assert.IsNull(result);
            result = _spaceService.GetSpace(childspace2.Id.ToString());
            Assert.IsNull(result);
            result = _spaceService.GetSpace(childspace_son.Id.ToString());
            Assert.IsNull(result);

            int count2 = _spaceService.GetAllSpaces().Count;
            Assert.AreEqual(count, count2);
        }

        [Test]
        public void should_pass_get_visilbity_spaces_process()
        {
            int count = _spaceService.GetAllSpaces().Count;

            string userId = Guid.NewGuid().ToString();
            string userId2 = Guid.NewGuid().ToString();
            string depId = Guid.NewGuid().ToString();

            var space = MakeSimpleSpace(userId,depId);
            var childspace = MakeChildSpace(space.Id.ToString(),userId, depId);
            var childspace2 = MakeChildSpace(childspace.Id.ToString(), userId2, depId);
            var childspace_son = MakeChildSpace(childspace2.Id.ToString(), userId2, depId);

            Assert.IsNotNull(_spaceService.Add(space));
            Assert.IsNotNull(_spaceService.Add(childspace));
            Assert.IsNotNull(_spaceService.Add(childspace2));
            Assert.IsNotNull(_spaceService.Add(childspace_son));

        
            space = TestVisiblity(space, Visible.Public, ref childspace, ref childspace2, ref childspace_son);

            var spaces = _spaceService.GetVisibleSpaces(userId, depId);
            int visiableCount = spaces.Count;
            Console.WriteLine("Before Set Visiable:" + visiableCount);
            Assert.IsTrue(visiableCount >= 4);

            _spaceService.SetVisiblity(childspace2.Id.ToString(), Visible.Private);

            spaces = _spaceService.GetVisibleSpaces(userId, depId);
            int visiableCount2 = spaces.Count;
            Console.WriteLine("After Set Visiable:" + visiableCount2);

            Assert.AreEqual(visiableCount - visiableCount2, 2);


            _spaceService.Delete(space.Id.ToString());
            var result = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsNull(result);
            result = _spaceService.GetSpace(childspace.Id.ToString());
            Assert.IsNull(result);
            result = _spaceService.GetSpace(childspace2.Id.ToString());
            Assert.IsNull(result);
            result = _spaceService.GetSpace(childspace_son.Id.ToString());
            Assert.IsNull(result);

            int count2 = _spaceService.GetAllSpaces().Count;
            Assert.AreEqual(count, count2);
        }

        private SpaceObject TestVisiblity(SpaceObject space, Visible visible, ref SpaceObject childspace,
            ref SpaceObject childspace2, ref SpaceObject childspace_son)
        {
            space = _spaceService.SetVisiblity(space.Id.ToString(), visible);
            space = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsTrue(space.Visible == (int) visible);

            childspace = _spaceService.GetSpace(childspace.Id.ToString());
            Assert.IsTrue(childspace.Visible == (int) visible);

            childspace2 = _spaceService.GetSpace(childspace2.Id.ToString());
            Assert.IsTrue(childspace2.Visible == (int) visible);

            childspace_son = _spaceService.GetSpace(childspace_son.Id.ToString());
            Assert.IsTrue(childspace_son.Visible == (int) visible);
            return space;
        }

        [Test]
        public void should_pass_move_and_recovery_delete_process()
        {
            int count = _spaceService.GetAllSpaces().Count;
            
            string userId = Guid.NewGuid().ToString();
            string depId = Guid.NewGuid().ToString();

            var space = MakeSimpleSpace(userId, depId);

            Assert.IsNotNull(_spaceService.Add(space));
            var result = _spaceService.GetSpace(space.Id.ToString());
            Assert.AreEqual(result.SpaceName, "技术空间");

            Assert.IsNotNull(_spaceService.ReName(space.Id.ToString(), "产品空间"));
            space = _spaceService.GetSpace(space.Id.ToString());
            Assert.AreEqual(space.SpaceName, @"产品空间");

            space = _spaceService.MoveToTrash(space.Id.ToString());
            space = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsTrue(space.IsDelete);

            space = _spaceService.Recovery(space.Id.ToString());
            space = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsFalse(space.IsDelete);

            _spaceService.Delete(space.Id.ToString());
            result = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsNull(result);

            int count2 = _spaceService.GetAllSpaces().Count;
            Assert.AreEqual(count, count2);
        }

        [Test]
        public void should_pass_move_recovery_child_spaces_process()
        {
            int count = _spaceService.GetAllSpaces().Count;

            string userId = Guid.NewGuid().ToString();
            string depId = Guid.NewGuid().ToString();

            var space = MakeSimpleSpace(userId,depId);

            var childspace = MakeChildSpace(space.Id.ToString(), userId, depId);
            var childspace2 = MakeChildSpace(childspace.Id.ToString(), userId, depId);
            var childspace_son = MakeChildSpace(childspace2.Id.ToString(), userId, depId);

            Assert.IsNotNull(_spaceService.Add(space));
            Assert.IsNotNull(_spaceService.Add(childspace));
            Assert.IsNotNull(_spaceService.Add(childspace2));
            Assert.IsNotNull(_spaceService.Add(childspace_son));

            space = _spaceService.MoveToTrash(space.Id.ToString());
            space = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsTrue(space.IsDelete);
            childspace = _spaceService.GetSpace(childspace.Id.ToString());
            Assert.IsTrue(childspace.IsDelete);
            childspace2 = _spaceService.GetSpace(childspace2.Id.ToString());
            Assert.IsTrue(childspace2.IsDelete);
            childspace_son = _spaceService.GetSpace(childspace_son.Id.ToString());
            Assert.IsTrue(childspace_son.IsDelete);

            space = _spaceService.Recovery(space.Id.ToString());
            space = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsFalse(space.IsDelete);
            childspace = _spaceService.GetSpace(childspace.Id.ToString());
            Assert.IsFalse(childspace.IsDelete);
            childspace2 = _spaceService.GetSpace(childspace2.Id.ToString());
            Assert.IsFalse(childspace2.IsDelete);
            childspace_son = _spaceService.GetSpace(childspace_son.Id.ToString());
            Assert.IsFalse(childspace_son.IsDelete);

            _spaceService.Delete(space.Id.ToString());
            var result = _spaceService.GetSpace(space.Id.ToString());
            Assert.IsNull(result);
            result = _spaceService.GetSpace(childspace.Id.ToString());
            Assert.IsNull(result);
            result = _spaceService.GetSpace(childspace2.Id.ToString());
            Assert.IsNull(result);
            result = _spaceService.GetSpace(childspace_son.Id.ToString());
            Assert.IsNull(result);

            int count2 = _spaceService.GetAllSpaces().Count;
            Assert.AreEqual(count, count2);
        }

        [Test]
        public void should_go_through_MakeSpace_process()
        {
            var space = _spaceService.MakeSpace(string.Empty, "001_西山区|4567_虹桥立交|8889_日常巡检", Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 0);
            Assert.AreEqual(space.SpaceName, "日常巡检");
            Assert.AreEqual(space.SpaceSeqNo, "8889");
        }

        [Test]
        public void should_go_through_MakeSpace_process2()
        {
            var space = _spaceService.MakeSpace(string.Empty, "001_西山区", Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 0);
            Assert.AreEqual(space.SpaceName, "西山区");
            Assert.AreEqual(space.SpaceSeqNo, "001");

        }

        private static SpaceObject MakeChildSpace(string parentId, string userId, string depId)
        {
            var childspace = new SpaceObject
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                CreateUserId = userId,
                DepId = depId,
                CreateUserName = "kane",
                SpaceName = "技术空间",
                FileCount = 1,
                ParentId = parentId,
                SpaceSeqNo = Guid.NewGuid().ToString(),
                SpaceSize = 1024,
            };
            return childspace;
        }

        private static SpaceObject MakeSimpleSpace(string userId, string depId)
        {
            var space = new SpaceObject
            {
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                CreateUserId = userId,
                CreateUserName = "kane",
                SpaceName = "技术空间",
                DepId = depId,
                SpaceSeqNo = Guid.NewGuid().ToString(),
                FileCount = 1,
                ParentId = Guid.NewGuid().ToString(),
                SpaceSize = 1024,
            };
            return space;
        }
    }
}
