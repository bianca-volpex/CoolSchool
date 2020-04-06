using System;
using System.IO;
using Common;
using CoolStudents.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static CoolStudents.Models.w2ui;
using DataContext.Models;
using DataContext;
using System.Linq;
using System.Collections.Generic;

namespace CoolStudents.Controllers
{
    public class StudentController : Controller
    {
        private readonly SchoolDB _db;

        public StudentController(SchoolDB context)
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

                var search = "";
                var searchField = "Id";
                var searchType = "int";
                var sortdir = "asc";
                var sortfield = "LastName";

                var specialFilters = new List<string>(){ "gender","group" };
                var genderFilter = false;
                var groupFilter = false;
                var specVal = "";

                if (postModel.search != null)
                {
                    var s = postModel.search[0];

                    if (!specialFilters.Contains(s.field))
                    {
                        search = s.value;
                        searchField = s.field;
                        searchType = s.type;
                    }
                    else
                    {
                        specVal = s.value;
                        genderFilter = s.field == "gender";
                        groupFilter = s.field == "group";
                    }

                }

                if (postModel.sort != null)
                {
                    sortdir = postModel.sort[0].direction;
                    sortfield =  postModel.sort[0].field;
                }

                SearchOptions options = null;

                if (searchField != "groups")
                    options =
                   new SearchOptions(sortfield, searchField, searchType)
                   .Limit(postModel.offset, postModel.limit)
                   .Sort(sortdir)
                   .Search(search);
                else
                    options =
                  new SearchOptions(sortfield, searchField, searchType)
                  .Limit(postModel.offset, postModel.limit)
                  .Sort(sortdir);

                w2uiGridModel<StudentAPI> model = new w2uiGridModel<StudentAPI>();


                var allStudents = _db.Students
                    .Where(el => genderFilter == false || el.Gender.Title == specVal)
                    .Where(el => groupFilter == false || el.UserInGroups.Select(gr=> gr.Group).Where(gr=> gr.Title == specVal).Any())
                    .ApplyFilterByType(options);

                model.records = allStudents
                    .ApplyPaging(options)
                    .Select(el => new StudentAPI
                    {
                        CallSign = el.CallSign,
                        FirstName = el.FirstName,
                        Gender = el.Gender,
                        GenderId = el.GenderId,
                        Groups = _db.UserInGroups
                    .Where(g => g.UserId == el.Id)
                    .Select(g => new IdText { id = g.Id, text = g.Group.Title })
                    .ToList(),
                        Id = el.Id,
                        LastName = el.LastName,
                        SecondName = el.SecondName,
                        UserInGroups = el.UserInGroups
                    })    
                    .ToList();

                model.total = allStudents.Count();

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

                w2uiJsonFormPostModel<StudentAPI> model = null;
                try
                {
                    model = JsonConvert.DeserializeObject<w2uiJsonFormPostModel<StudentAPI>>(json);
                }

                catch (Exception ex)
                {
                    return Json(w2uiBaseModel.Error(ex));
                }

                if (model.cmd == "save-record")
                {
                    var record = model.record;

                    //чтобы не записывалась пустая строка
                    if (string.IsNullOrEmpty(record.CallSign))
                        record.CallSign = null;

                    if (!TryValidateModel(record, nameof(Student))) {
                        var errors = ModelState
                            .Values
                            .Select(el  => el.Errors.Select(m=> m.ErrorMessage).FirstOrDefault() )
                            .ToList();

                        var msg = string.Join(", ", errors);
                        return Json(w2uiBaseModel.Error(msg));
                    };
                  

                    var st = _db.Students.FirstOrDefault(el => el.Id == record.Id);
                    if (st == null) throw new Exception("Не найден student");

                    var oldGroupList = _db
                        .UserInGroups
                        .Where(el => el.UserId == st.Id)
                        .Select(el => el.GroupId)
                        .ToList();

                    var newGroupList = record.Groups.Select(el => el.id).ToList();

                    //на добавление
                    var forAddIds = newGroupList.Except(oldGroupList).ToList();
                    var forAdd = forAddIds.Select(
                        el => new UserInGroup
                        {
                            GroupId = el,
                            UserId = record.Id
                        })
                    .ToList();

                    //на удаление
                    var forDelIds = oldGroupList.Except(newGroupList).ToList();
                    var forDel = _db.UserInGroups.Where(el => forDelIds.Contains(el.GroupId));

                    st.FirstName = record.FirstName;
                    st.GenderId = record.GenderId;
                    st.LastName = record.LastName;
                    st.SecondName = record.SecondName;
                    st.CallSign = record.CallSign;

                    _db.SaveChanges();

                    _db.UserInGroups.AddRange(forAdd);
                    _db.UserInGroups.RemoveRange(forDel);

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
                var newSt = new Student();
                _db.Students.Add(newSt);
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
                var st = _db.Students.FirstOrDefault(el => el.Id == id);
                if (st == null) throw new Exception("Не найден student");

                _db.Students.Remove(st);
                _db.SaveChanges();

                return Json(w2uiBaseModel.Ok());
            }
            catch (Exception ex)
            {
                return Json(w2uiBaseModel.Error(ex));
            }
        }

        [HttpGet]
        public ActionResult Genders()
        {
            try
            {
                w2uiItemsModel<IdText> res = new w2uiItemsModel<IdText>();

                res.items = _db.Gender
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
    }
}
