using Microsoft.AspNetCore.Mvc;
using TodoMvcApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace TodoMvcApp.Controllers
{
    public class TodoController : Controller
    {
        // Display the list
        public IActionResult Index()
        {
            var visibleItems = _context.TodoItem
                         .Where(item => !item.IsHidden)
                         .ToList();
            return View(visibleItems);
        }

        // GET: /Todo/Add
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Todo/Add
        [HttpPost]
        public IActionResult Add(TodoItem newItem)
        {
            newItem.IsDone = false;
            newItem.IsHidden = false;

            _context.TodoItem.Add(newItem);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleDone(int id)
        {
            var item = _context.TodoItem.Find(id);
            if (item != null)
            {
                item.IsDone = !item.IsDone;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Hide(int id)
        {
            var item = _context.TodoItem.Find(id);
            if (item != null)
            {
                item.IsHidden = true;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Hidden()
        {
            var hiddenItems = _context.TodoItem.Where(item => item.IsHidden).ToList();
            return View(hiddenItems);
        }

        [HttpPost]
        public IActionResult Unhide(int id)
        {
            var item = _context.TodoItem.Find(id);
            if (item != null)
            {
                item.IsHidden = false;
                _context.SaveChanges();
            }
            return RedirectToAction("Hidden");
        }

        [HttpGet]
        public IActionResult Filter(string category, int? priority)
        {
            var filteredItems = _context.TodoItem.Where(item => !item.IsHidden);

            if (!string.IsNullOrEmpty(category))
            {
                filteredItems = filteredItems.Where(item => item.Category == category);
            }

            if (priority.HasValue)
            {
                filteredItems = filteredItems.Where(item => item.Priority == priority.Value);
            }

            return View("Index", filteredItems.ToList());
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var item = _context.TodoItem.Find(id);
            if (item != null)
            {
                _context.TodoItem.Remove(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        private readonly AppDbContext _context;

        public TodoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Sort(string sortBy, string order)
        {
            var items = _context.TodoItem.Where(t => !t.IsHidden);

            switch (sortBy)
            {
                case "title":
                    items = order == "asc" ? items.OrderBy(t => t.Title) : items.OrderByDescending(t => t.Title);
                    break;
                case "category":
                    items = order == "asc" ? items.OrderBy(t => t.Category) : items.OrderByDescending(t => t.Category);
                    break;
                case "priority":
                    items = order == "asc" ? items.OrderBy(t => t.Priority) : items.OrderByDescending(t => t.Priority);
                    break;
                case "status":
                    items = order == "asc" ? items.OrderBy(t => t.IsDone) : items.OrderByDescending(t => t.IsDone);
                    break;
            }

            return View("Index", items.ToList());
        }

        // GET: /Todo/Edit/{id}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var item = _context.TodoItem.FirstOrDefault(t => t.Id == id && !t.IsHidden);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /Todo/Edit/{id}
        [HttpPost]
        public IActionResult Edit(int id, TodoItem updated)
        {
            var item = _context.TodoItem.FirstOrDefault(t => t.Id == id && !t.IsHidden);
            if (item == null)
                return NotFound();

            item.Title = updated.Title;
            item.Category = updated.Category;
            item.Priority = updated.Priority;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}