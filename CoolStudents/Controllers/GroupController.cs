using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoolStudents.Models;
using DataContext;
using static CoolStudents.Models.w2ui;
using Common;
using DataContext.Models;
using Group = DataContext.Models.Group;
using System.IO;
using Newtonsoft.Json;

namespace CoolStudents.Controllers
{
    public class GroupController : Controller
    {
        private readonly SchoolDB _db;

        public GroupController(SchoolDB context)
        {
            _db = context;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult List()
        {
            try
            {
                w2uiGridJsonPostModel postModel = null;
                try
                {
                    postModel = RequestConverter.FromJsonRequest<w2uiGridJsonPostModel>(Request.Body);
                }
                catch (Exception ex)
                {
                    return Json(w2uiBaseModel.Error(ex));
                }

                var search = postModel.search != null ? postModel.search[0].value : "";
                var searchField = postModel.search != null ? postModel.search[0].field : "Title";
                var searchType = postModel.search != null ? postModel.search[0].type : "text";

                var sortdir = postModel.sort != null ? postModel.sort[0].direction : "asc";
                var sortfield = postModel.sort != null ? postModel.sort[0].field : "Title";

                SearchOptions options =
                    new SearchOptions(sortfield, searchField, searchType)
                    .Limit(postModel.offset, postModel.limit)
                    .Sort(sortdir)
                    .Search(search);

                w2uiGridModel<GroupAPI> model = new w2uiGridModel<GroupAPI>();


                var allGroups = _db
                    .Groups
                    .ApplyFilterByType(options);

                model.records = allGroups
                    .ApplyPaging(options)
                    .Select(el => new GroupAPI
                    {
                        Id = el.Id,
                        Title = el.Title,
                        UserCount = _db.UserInGroups.Where(g => g.GroupId == el.Id).Count()
                    })
                    .ToList();

                model.total = allGroups.Count();

                return Json(model);

            }
            catch (Exception ex)
            {
                return Json(w2uiBaseModel.Error(ex));
            }
        }

        [HttpPost]
        public ActionResult Info()
        {
            try
            {
                Stream req = Request.Body;
                string json = new StreamReader(req).ReadToEnd();

                w2uiJsonFormPostModel<Group> model = null;
                try
                {
                    model = JsonConvert.DeserializeObject<w2uiJsonFormPostModel<Group>>(json);
                }

                catch (Exception ex)
                {
                    return Json(w2uiBaseModel.Error(ex));
                }

                if (model.cmd == "save-record")
                {
                    if (!TryValidateModel(model.record, nameof(Student)))
                    {
                        var errors = ModelState
                            .Values
                            .Select(el => el.Errors.Select(m => m.ErrorMessage).FirstOrDefault())
                            .ToList();

                        var msg = string.Join(", ", errors);
                        return Json(w2uiBaseModel.Error(msg));
                    };

                    var record = model.record;

                    var gr = _db.Groups.FirstOrDefault(el => el.Id == record.Id);
                    if (gr == null) throw new Exception("Не найдена группа");

                    gr.Title = record.Title;

                    _db.SaveChanges();

                }

                return Json(new w2uiBaseModel());
            }
            catch (Exception ex)
            {
                return Json(w2uiBaseModel.Error(ex));
            }
        }

        [HttpPost]
        public ActionResult Add()
        {
            try
            {
                var newGr = new Group();
                _db.Groups.Add(newGr);
                _db.SaveChanges();

                return Json(w2uiBaseModel.Ok());
            }
            catch (Exception ex)
            {
                return Json(w2uiBaseModel.Error(ex));
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                var gr = _db.Groups.FirstOrDefault(el => el.Id == id);
                if (gr == null) throw new Exception("Не найдена группа");

                _db.Groups.Remove(gr);
                _db.SaveChanges();

                return Json(w2uiBaseModel.Ok());
            }
            catch (Exception ex)
            {
                return Json(w2uiBaseModel.Error(ex));
            }
        }

        [HttpGet]
        public ActionResult GroupList(int id)
        {
            try
            {
                w2uiItemsModel<IdText> res = new w2uiItemsModel<IdText>();

                res.items = _db.Groups
                   .Select(el => new IdText
                   {
                       id = el.Id,
                       text = el.Title
                   })
               .ToList();

                return Json(res);
            }
            catch (Exception ex)
            {
                return Json(w2uiBaseModel.Error(ex));
            }
        }

        [HttpGet]
        public ActionResult Groups()
        {
            try
            {
                w2uiItemsModel<IdText> res = new w2uiItemsModel<IdText>();

                res.items = _db.Groups
                   .Select(el => new IdText
                   {
                       id = el.Id,
                       text = el.Title
                   })
                   .OrderBy(el => el.text)
               .ToList();

                return Json(res);
            }
            catch (Exception ex)
            {
                return Json(w2uiBaseModel.Error(ex));
            }
        }
    }
}
