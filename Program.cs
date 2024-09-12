using System.Text.Json;
using TaskTrackerCLIApp.Models;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        // Define the path to store tasks.json file
        string filePath = "tasks.json";

        if (args.Length > 0)
        {
            string command = args[0].ToLower();

            if (command == "add-task" && args.Length > 1)
            {
                string taskDescription = string.Join(' ', args, 1, args.Length - 1);
                AddTask(filePath, taskDescription);
            }
            else if (command == "update-task-in-progress" && args.Length > 1)
            {
                string taskDescription = string.Join(' ', args, 1, args.Length - 1);
                UpdateTaskInProgress(filePath, taskDescription);
            }
            else if (command == "update-task-done" && args.Length > 1)
            {
                string taskDescription = string.Join(' ', args, 1, args.Length - 1);
                UpdateTaskDone(filePath, taskDescription);
            }
            else if (command == "get-tasks")
            {
                GetAllTasks(filePath);
            }
            else if (command == "help")
            {
                ShowHelp();
            }
        }
        else
        {
            ShowHelp();
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("Usage: TaskTrackerCLIApp [command] [options]");
        Console.WriteLine("Commands:");
        Console.WriteLine("  help       Show this help message.");
        Console.WriteLine("  add-task     Adding new task by task description.");
        Console.WriteLine("  update-task-in-progress  Update the existing task to in progress status.");
        Console.WriteLine("  update-task-done  Update the existing task to done status.");
        Console.WriteLine("  get-tasks    Get all tasks.");
    }

    #region GET
    static void GetAllTasks(string filePath)
    {
        // Read existing tasks from the file
        List<TaskItem> tasks = ReadAllTasks(filePath);

        if (tasks.Count == 0)
            Console.WriteLine($"All task: There is no task!");

        Console.WriteLine($"All task: {tasks.Count}");
        Console.WriteLine(new string('-', 50));

        foreach (var item in tasks)
        {
            Console.WriteLine($"Task: {item.Description}");
            Console.WriteLine($"Status: {item.Status}");
            Console.WriteLine($"Created: {item.CreatedAt}");
            Console.WriteLine($"Updated: {item.UpdatedAt?.ToString() ?? "Not updated"}");
            Console.WriteLine(new string('-', 40));
        }
    }
    #endregion

    #region ADD
    static void AddTask(string filePath, string taskDescription)
    {
        // Create a task object
        TaskItem newTask = new()
        {
            Id = Guid.NewGuid(),
            Description = taskDescription,
            Status = TaskItemStatus.ToDo,
            CreatedAt = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss")
        };

        // Read existing tasks from the file
        List<TaskItem> tasks = [];
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            tasks = JsonSerializer.Deserialize<List<TaskItem>>(json)!;
        }

        // Add new task
        tasks?.Add(newTask);

        // Save updated tasks to JSON file
        JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
        JsonSerializerOptions options = jsonSerializerOptions;
        string updatedJson = JsonSerializer.Serialize(tasks!, options);
        File.WriteAllText(filePath, updatedJson);

        Console.WriteLine($"Task added: \"{taskDescription}\"");
    }
    #endregion

    #region UPDATE
    static void UpdateTaskInProgress(string filePath, string taskDescription)
    {
        UpdateTask(filePath, taskDescription, TaskItemStatus.InProgress);
    }

    static void UpdateTaskDone(string filePath, string taskDescription)
    {
        UpdateTask(filePath, taskDescription, TaskItemStatus.Done);
    }

    static void UpdateTask(string filePath, string taskDescription, string status)
    {
        List<TaskItem> tasks = ReadAllTasks(filePath);
        var taskItem = tasks.Find(x => x.Description!.Equals(taskDescription));

        if (taskItem is null)
            Console.WriteLine($"There no task with {taskDescription}.");

        taskItem!.Status = status;
        taskItem.UpdatedAt = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");

        WriteTasksToJsonFile(filePath, tasks);

        Console.WriteLine($"Updated successfully {taskDescription}.");
    }
    #endregion

    #region Action Call
    static List<TaskItem> ReadAllTasks(string filePath)
    {
        List<TaskItem> tasks = [];
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            tasks = JsonSerializer.Deserialize<List<TaskItem>>(json)!;
        }
        return tasks;
    }

    public static void WriteTasksToJsonFile(string filePath, List<TaskItem> tasks)
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(tasks, options);
        File.WriteAllText(filePath, json);
    }
    #endregion
}
