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
        private static readonly string dataFilePath = "todoData.json";
        
        // In-memory list to store todo items
        private static List<TodoItem> todoList = new List<TodoItem>();

        // Display the list
        public IActionResult Index()
        {
            // Only show items that are not hidden
            var visibleItems = todoList.Where(item => !item.IsHidden).ToList();
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
            // Assign a unique ID manually since we have no DB
            newItem.Id = todoList.Count > 0 ? todoList.Max(i => i.Id) + 1 : 1;

            // Default values (if needed)
            newItem.IsDone = false;
            newItem.IsHidden = false;

            todoList.Add(newItem);
            SaveData();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleDone(int id)
        {
            var item = todoList.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                item.IsDone = !item.IsDone;
                SaveData();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Hide(int id)
        {
            var item = todoList.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                item.IsHidden = true;
                SaveData();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Hidden()
        {
            var hiddenItems = todoList.Where(item => item.IsHidden).ToList();
            return View(hiddenItems);
        }

        [HttpPost]
        public IActionResult Unhide(int id)
        {
            var item = todoList.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                item.IsHidden = false;
                SaveData();
            }

            return RedirectToAction("Hidden");
        }

        [HttpGet]
        public IActionResult Filter(string category, int? priority)
        {
            var filteredItems = todoList.Where(item => !item.IsHidden);

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
            var item = todoList.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                todoList.Remove(item);
                SaveData();
            }

            return RedirectToAction("Index");
        }

        private static void SaveData()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true // 👈 This enables line breaks and indentation
            };

            string json = JsonSerializer.Serialize(todoList, options);
            System.IO.File.WriteAllText(dataFilePath, json);
        }

        private static void LoadData()
        {
            if (System.IO.File.Exists(dataFilePath))
            {
                var json = System.IO.File.ReadAllText(dataFilePath);
                todoList = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
            }
        }

        public TodoController()
        {
            if (todoList.Count == 0)
            {
                LoadData();
            }
        }

        [HttpGet]
        public IActionResult Sort(string sortBy, string order)
        {
            var items = todoList.Where(t => !t.IsHidden);

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
            var item = todoList.FirstOrDefault(t => t.Id == id && !t.IsHidden);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // POST: /Todo/Edit/{id}
        [HttpPost]
        public IActionResult Edit(int id, TodoItem updated)
        {
            var item = todoList.FirstOrDefault(t => t.Id == id && !t.IsHidden);
            if (item == null)
                return NotFound();

            // Update fields
            item.Title = updated.Title;
            item.Category = updated.Category;
            item.Priority = updated.Priority;

            SaveData();

            return RedirectToAction("Index");
        }
    }
}