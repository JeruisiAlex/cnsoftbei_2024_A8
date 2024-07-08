cmake_minimum_required(VERSION 3.28)
project(cnsoftbei_2024_A8 C)

set(CMAKE_C_STANDARD 11)

# 添加 gtk3.0
find_package(PkgConfig REQUIRED)
pkg_check_modules(GTK3 REQUIRED gtk+-3.0)
pkg_check_modules(FREERDP REQUIRED freerdp2)
pkg_check_modules(FREERDP_CLIENT REQUIRED freerdp-client2)
include_directories(${GTK3_INCLUDE_DIRS} ${FREERDP_INCLUDE_DIRS} ${FREERDP_CLIENT_INCLUDE_DIRS})
link_directories(${GTK3_LIBRARY_DIRS} ${FREERDP_LIBRARY_DIRS} ${FREERDP_CLIENT_LIBRARY_DIRS})
list(APPEND FC_DEP_LIBS ${GTK3_LIBRARIES})
#


add_executable(cnsoftbei_2024_A8
        main.c
        include/ui.h
        src/UI/frame.c
        src/UI/bar.c
        src/UI/content.c
        include/err.h
        include/info.h
        src/info/basic.c
)

#
target_link_libraries (cnsoftbei_2024_A8 ${FC_DEP_LIBS} ${FREERDP_LIBRARIES} ${FREERDP_CLIENT_LIBRARIES})
