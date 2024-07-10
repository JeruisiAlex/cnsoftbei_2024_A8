cmake_minimum_required(VERSION 3.28)
project(cnsoftbei_2024_A8 C)

set(CMAKE_C_STANDARD 11)

# 添加 gtk3.0
find_package(PkgConfig REQUIRED)
pkg_check_modules(GTK3 REQUIRED gtk+-3.0)
include_directories(${GTK3_INCLUDE_DIRS})
link_directories(${GTK3_LIBRARY_DIRS})
list(APPEND FC_DEP_LIBS ${GTK3_LIBRARIES})
#


add_executable(cnsoftbei_2024_A8
        main.c
        include/ui.h
        include/err.h
        src/UI/frame.c
        src/UI/bar.c
        src/UI/content.c
        include/network.h
        include/kernel.h
        src/kernel.c
        src/network.c
        src/UI/func.c
        include/uifunc.h
)

#
target_link_libraries (cnsoftbei_2024_A8 ${FC_DEP_LIBS})

