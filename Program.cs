using Gtk;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static string dir;
    static List<string> attributes;
    static string idsPath;
    static List<string> pageIds;
    static Window window;

    static void Main(string[] args)
    {
        Application.Init();
        window = new Window("Page List");
        window.SetDefaultSize(500, 500);
        window.DeleteEvent += (o, args) => Application.Quit();

        // Get directory from command line argument
        if (args.Length < 1)
        {
            ShowError("Usage: SimpleGuiApp <directory_with_props.txt>");
            Application.Quit();
            return;
        }
        dir = args[0];
        string propsPath = Path.Combine(dir, "props.txt");
        idsPath = Path.Combine(dir, "ids.txt");
        attributes = new List<string>();
        if (File.Exists(propsPath))
        {
            foreach (var line in File.ReadAllLines(propsPath))
            {
                var trimmed = line.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    attributes.Add(trimmed);
            }
        }
        else
        {
            ShowError($"File not found: {propsPath}");
            Application.Quit();
            return;
        }
        if (!File.Exists(idsPath))
        {
            File.WriteAllText(idsPath, "");
        }
        ShowPageList();
        Application.Run();
    }

    static void ShowError(string message)
    {
        var dialog = new MessageDialog(
            window,
            DialogFlags.Modal,
            MessageType.Error,
            ButtonsType.Ok,
            message);
        dialog.Title = "Error";
        dialog.Run();
        dialog.Destroy();
    }

    static void ShowInfo(string message)
    {
        var dialog = new MessageDialog(
            window,
            DialogFlags.Modal,
            MessageType.Info,
            ButtonsType.Ok,
            message);
        dialog.Title = "Info";
        dialog.Run();
        dialog.Destroy();
    }

    static void ShowPageList()
    {
        // Read page IDs
        pageIds = File.ReadAllLines(idsPath)
        .Select(l => l.Trim())
        .Where(l => !string.IsNullOrEmpty(l))
        .ToList();

        var vbox = new VBox(false, 8) { BorderWidth = 10 };
        var scrolled = new ScrolledWindow();
        var listVBox = new VBox(false, 4);

        foreach (var pageId in pageIds)
        {
            var hbox = new HBox(false, 4);
            var editBtn = new Button("Edit");
            var delBtn = new Button("Delete");
            editBtn.WidthRequest = 60;
            delBtn.WidthRequest = 60;
            hbox.PackStart(editBtn, false, false, 0);
            hbox.PackStart(delBtn, false, false, 0);

            // Get title
            string title = pageId;
            string titlePath = Path.Combine(dir, $"{pageId}-title.txt");
            if (File.Exists(titlePath))
            {
                var t = File.ReadAllText(titlePath).Trim();
                if (!string.IsNullOrEmpty(t))
                    title = t;
            }
            var label = new Label(title) { Xalign = 0 };
            hbox.PackStart(label, true, true, 0);

            editBtn.Clicked += (s, e) => ShowEditPage(pageId, false);
            delBtn.Clicked += (s, e) => {
                // Remove from ids.txt
                pageIds.Remove(pageId);
                File.WriteAllLines(idsPath, pageIds);
                // Delete all attribute files
                foreach (var attr in attributes)
                {
                    string f = Path.Combine(dir, $"{pageId}-{attr}.txt");
                    if (File.Exists(f)) File.Delete(f);
                }
                ShowPageList();
            };
            listVBox.PackStart(hbox, false, false, 0);
        }
        scrolled.AddWithViewport(listVBox);
        vbox.PackStart(scrolled, true, true, 0);

        // Add new page button
        var addBtn = new Button("Add new page");
        addBtn.Clicked += (s, e) => ShowEditPage("", true);
        vbox.PackStart(addBtn, false, false, 0);

        window.Remove(window.Child);
        window.Add(vbox);
        window.ShowAll();
    }

    static void ShowEditPage(string pageId, bool isNew)
    {
        var vbox = new VBox(false, 8) { BorderWidth = 10 };
        var inputWidgets = new Dictionary<string, Widget>();
        Entry idEntry = null;
        if (isNew)
        {
            var idLabel = new Label("ID:");
            idEntry = new Entry();
            vbox.PackStart(idLabel, false, false, 0);
            vbox.PackStart(idEntry, false, false, 0);
        }
        foreach (var attr in attributes)
        {
            var label = new Label(attr.Substring(0, 1).ToUpper() + attr.Substring(1) + ":");
            vbox.PackStart(label, false, false, 0);
            if (attr.ToLower() == "body")
            {
                var textView = new TextView();
                textView.WrapMode = WrapMode.Word;
                textView.SetSizeRequest(380, 150);
                if (!isNew && File.Exists(Path.Combine(dir, $"{pageId}-{attr}.txt")))
                    textView.Buffer.Text = File.ReadAllText(Path.Combine(dir, $"{pageId}-{attr}.txt"));
                    vbox.PackStart(textView, true, true, 0);
                    inputWidgets[attr] = textView;
            }
            else
            {
                var entry = new Entry();
                if (!isNew && File.Exists(Path.Combine(dir, $"{pageId}-{attr}.txt")))
                    entry.Text = File.ReadAllText(Path.Combine(dir, $"{pageId}-{attr}.txt"));
                    vbox.PackStart(entry, false, false, 0);
                    inputWidgets[attr] = entry;
            }
        }
        var hbox = new HBox(false, 8);
        var saveBtn = new Button("Save");
        var cancelBtn = new Button("Cancel");
        hbox.PackStart(saveBtn, false, false, 0);
        hbox.PackStart(cancelBtn, false, false, 0);
        vbox.PackStart(hbox, false, false, 0);

        saveBtn.Clicked += (s, e) => {
            string newId = isNew ? idEntry.Text.Trim() : pageId;
            if (string.IsNullOrEmpty(newId))
            {
                ShowError("ID cannot be empty");
                return;
            }
            if (isNew && pageIds.Contains(newId))
            {
                ShowError("ID already exists");
                return;
            }
            foreach (var attr in attributes)
            {
                string value = "";
                if (inputWidgets[attr] is Entry entry)
                    value = entry.Text;
                else if (inputWidgets[attr] is TextView textView)
                    value = textView.Buffer.Text;
                File.WriteAllText(Path.Combine(dir, $"{newId}-{attr}.txt"), value);
            }
            if (isNew)
            {
                pageIds.Add(newId);
                File.WriteAllLines(idsPath, pageIds);
            }
            ShowPageList();
        };
        cancelBtn.Clicked += (s, e) => ShowPageList();

        window.Remove(window.Child);
        window.Add(vbox);
        window.ShowAll();
    }
}
