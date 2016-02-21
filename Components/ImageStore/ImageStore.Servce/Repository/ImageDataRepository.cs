using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common.Logging;

namespace ImageStore.Services.Repository
{
    public class ImageDataRepository
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Add(Images entity)
        {
            try
            {
                using (var db = new DataImageDataContext())
                {
                    var table = db.GetTable<Images>();
                    table.InsertOnSubmit(entity);
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

        }
        public void Update(Images entity)
        {
            try
            {
                using (var db = new DataImageDataContext())
                {
                    var oldEntity = db.GetTable<Images>().First(p => p.Id == entity.Id);
                    oldEntity.Name = entity.Name;
                    oldEntity.ImageUrl = entity.ImageUrl;
                    oldEntity.CompressUrl = entity.CompressUrl;
                    oldEntity.ThumbUrl = entity.ThumbUrl;
                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

        }

        public Images Get(Expression<Func<Images, bool>> condition)
        {

            try
            {
                using (var db = new DataImageDataContext())
                {
                    return db.GetTable<Images>().FirstOrDefault(condition);
                }
            }
            catch (Exception ex)
            {

                _logger.Error(ex.Message);
            }
            return null;
        }

        public List<Images> GetImages(Expression<Func<Images, bool>> condition)
        {
            var list = new List<Images>();

            try
            {
                using (var db = new DataImageDataContext())
                {
                    list = db.GetTable<Images>().Where(condition).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            return list;
        }
    }
}
