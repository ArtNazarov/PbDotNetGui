# PbDotNetGui
GUI for https://github.com/ArtNazarov/pb written on C# .Net

# Overview

This tool provides a graphical interface for managing content that will be used to generate a static website. 
It's built with C# and GTK# for cross-platform compatibility.

# Screenshots

![Screen 1](https://dl.dropbox.com/scl/fi/x01n2w1udxbbhbmir6nbz/pbgui.png?rlkey=92smbll6otanbh1uhqjhjasdz&st=i0wiq4br)
![Screen 2](https://dl.dropbox.com/scl/fi/0in1d5k7v2hnkxvrnrou0/pbgui2.png?rlkey=7ud84k0rxy50zlycjhc5o32kf&st=l67m297u)
![Screen 3](https://dl.dropbox.com/scl/fi/yu6rbe7wpjp1z3nvycwxn/pbgui3.png?rlkey=bido88ujig40kpye6fxly3yuy&st=vnlts0gv)
![Screen 4](https://dl.dropbox.com/scl/fi/g8vnpelxjzqpn6vmjcjzf/pbgui4.png?rlkey=a5l4dri05bx41s8v1brskcg4j&st=emisfzn1)

# Key Features

Content Management GUI: Intuitive interface for adding, editing, and deleting pages
Attribute-Based System: Flexible content structure defined in props.txt
File-Based Storage: All content stored in simple text files
Cross-Platform: Runs on Windows, Linux, and macOS via GTK#

# File Structure

The system uses several key files:

- props.txt: Defines the attributes/properties each page can have (e.g., "title", "body")

- ids.txt: Contains the list of all page IDs

- ID-ATTRIBUTE.txt: Stores attribute values for each page (e.g., "about-body.txt")

- template.txt: HTML template file with placeholders for attributes (not shown in GUI but used for generation)

# Prerequisites

.NET Core 3.1 or later

GTK# libraries

# Before installation

```
sudo pacman -S dotnet-sdk
sudo pacman -S gtk-sharp-2
```

# Dependencies

In project directory execute

```
dotnet add package GtkSharp --version 3.24.24.36
```

# Launching app

```
dotnet run <directory_with_props.txt>
```
or goto `/bin/Debug/net9.0` and execute:

```
 ./PbDotNetGui <directory_with_props.txt>
```

# Application Workflow

Initialize Content Directory:
Create a directory with props.txt defining your page attributes
The GUI will automatically create ids.txt if it doesn't exist

# Manage Pages:
Add new pages with unique IDs
Edit existing pages to update their attributes
Delete pages when no longer needed

# Content Files:
Each attribute defined in props.txt will generate a corresponding text file for each page
Special handling for "body" attributes (uses multi-line text input)

# Building the Site

While the GUI focuses on content management, 
the actual site generation would be handled by a separate tool [generator](https://github.com/ArtNazarov/pb) that would:
- Read the same directory structure
- Process template.txt with the content files
- Generate HTML files in a build directory

# Performance Considerations

The GUI is designed for content management rather than high-volume generation. For actual site building:
- The C++ generator can handle ~1.3ms per page (100,000 pages in ~130 seconds)
- Consider separating content editing from site generation in your workflow

# Development Notes

The application follows these architectural patterns:

MVC-like separation: GUI (View), file operations (Model), and application logic (Controller)
Event-driven design: GTK# signals handle user interactions
File-based persistence: All changes immediately written to disk

# TODO

Extending the System to add new features:
- Additional Attributes: Simply add them to props.txt
- Custom Field Types: Extend the ShowEditPage method to handle new input types
- Template Management: Could be added as a future feature in the GUI

# LICENSE

MIT

# Author

Nazarov A.A., Orenburg. Russia, 2025
