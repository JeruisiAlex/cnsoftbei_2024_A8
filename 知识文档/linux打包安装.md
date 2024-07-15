# 主要流程

1. **安装必要工具**：

   - 在你的系统上安装`dpkg-dev`和`build-essential`工具包。这些工具包包括了创建和管理`.deb`包所需的基本工具和库。

   - 使用以下命令安装：

     ```shell
     sudo apt-get update
     sudo apt-get install dpkg-dev build-essential
     ```

2. **设置目录结构**：

   - 确保你的项目目录结构正确。例如，对于一个名为

     ```
     example
     ```

     的项目，目录结构可能如下：

     ```
     example-1.0/
     ├── DEBIAN/
     │   ├── control
     |   ├── postinst
     │   └── install
     ├── usr/
     │   ├── local/
     │   │   └── bin/
     │   │       └── example (your binary or script)
     │   └── share/
     │       └── applications/
     │           └── example.desktop
     │       └── icons/
     │           └── example
     |             └──example.png (your application icon)
     ```

> 除了源码之外的文件，会在后面的“其他文件”中提到写的方法。

3. **设置文件权限**：

- 确保你的文件和目录具有正确的权限。通常，所有文件应具有755权限，控制文件应具有644权限。

- 使用以下命令设置权限：

  ```
  chmod 755 example-1.0/usr/local/bin/example
  chmod 644 example-1.0/DEBIAN/control
  chmod 644 example-1.0/usr/share/applications/example.desktop
  chmod 644 example-1.0/usr/share/icons/example.png
  ```

> 对于权限的解释，见下面的“权限解释”

5. **构建包**：

- 使用

  ```
  dpkg-deb
  ```

  命令来构建包：

  ```
  dpkg-deb --build example-1.0
  ```

6. **测试包**：

- 构建完成后，你可以使用

  ```
  dpkg
  ```

  命令来安装和测试包：

  ```
  sudo dpkg -i example-1.0.deb
  ```
  
- 检查安装是否成功，并确保你的应用程序按预期工作。

## 其他文件

### control

``` vbnet
Package: example
Version: 1.0
Section: base
Priority: optional
Architecture: amd64
Maintainer: Your Name <your.email@example.com>
Description: An example package
```

**Section**: `base`

- 这个字段用来指定软件包所属的分类或部分。在 Debian 系统中，软件包被划分到不同的部分（Sections）中，比如 `base`, `devel`, `utils` 等。`base` 部分通常包含了基本的系统工具和库。

**Priority**: `optional`

- 这个字段指定了软件包的优先级。软件包可以有不同的优先级，比如 `required`, `important`, `standard`, `optional`, `extra` 等。这个优先级决定了在安装时软件包的处理顺序和重要性。

**Architecture**: `amd64`

- 这个字段指定了软件包适用的硬件架构。软件包可以编译为多种硬件架构，比如 `amd64`（64位的x86架构）、`i386`（32位的x86架构）、`armhf`（ARM架构的硬浮点支持版本）等。

### .desktop

``` makefile
[Desktop Entry]
Name=Example Application
Comment=This is an example application
Exec=/usr/local/bin/example
Icon=/usr/local/share/icons/example.png
Terminal=false
Type=Application
Categories=Utility;
```

`[Desktop Entry]`：

- 这是必须的，标识这个文件是一个桌面入口文件。

`Name=Example Application`：

- `Name`是应用程序的名称，会显示在菜单栏中。

`Comment=This is an example application`：

- `Comment`是应用程序的简短描述，会显示在工具提示中（当你悬停在应用程序图标上时）。

`Exec=/usr/local/bin/example`：

- `Exec`指定了启动应用程序的命令。这里可以是可执行文件的路径，也可以是包含参数的命令。示例：`Exec=/usr/local/bin/example --option1 --option2`。

`Icon=/usr/local/share/icons/example.png`：

- `Icon`指定了应用程序图标的路径。图标路径可以是绝对路径，也可以是仅文件名（如果图标位于系统图标目录中）。

`Terminal=false`：

- `Terminal`指定应用程序是否在终端中运行。如果设置为`true`，应用程序将在终端窗口中启动。通常GUI应用程序设置为`false`。

`Type=Application`：

- `Type`指定桌面入口的类型。常见的类型有`Application`（应用程序）、`Link`（链接）和`Directory`（目录）。

`Categories=Utility;`：

- `Categories`定义应用程序所属的类别。可以包含多个类别，用分号分隔。常见类别有`Utility`（工具）、`Development`（开发）、`Education`（教育）等。这有助于桌面环境将应用程序分类到正确的菜单中。

### install

在这个文件中，你可以指定源目录中的文件应该被安装到目标系统的哪个目录。

格式通常是：`源路径 目标路径`。

例如:

``` c
bin/myapp usr/bin
```

这表示将 `bin/myapp` 安装到 `/usr/bin` 目录。

### postinst

这个脚本会在安装后执行。我这里用来创建桌面快捷方式。

``` shell
#!/bin/sh
set -e

case "$1" in
    configure)
        # Create a desktop shortcut for the application
        cp /usr/share/applications/yourapp.desktop /usr/share/desktop-directories/yourapp.desktop
        ;;
    *)
        # Do nothing if not in configure phase
        ;;
esac

# Exit successfully
exit 0
```

记得更改路径。和确保该脚本的权限

``` shell
chmod +x postinst
```

## 权限解释

举例：

- **6** （文件所有者的权限）：这个数字代表读（4）和写（2）权限，加起来为6。所有者可以读取和修改文件。
- **6** （同组用户的权限）：这个数字同样代表读（4）和写（2）权限，加起来为6。同一用户组内的其他用户也可以读取和修改文件。
- **4** （其他用户的权限）：这个数字代表只读（4）权限。系统上的其他用户只能读取文件，不能修改。

详解：

- **读（4）**：允许读取文件内容。
- **写（2）**：允许修改文件内容。
- **执行（1）**：允许执行文件（通常用于脚本和程序）。

> +x：可执行权限

## 注意事项

* 如果代码使用了路径，记得更改
