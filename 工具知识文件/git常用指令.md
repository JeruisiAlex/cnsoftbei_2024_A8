### 初始化环境

- git clone https://github.com/JeruisiAlex/cnsoftbei_2024_A8.git
- cd cnsoftbei_2024_A8
- git fetch
- git branch -r：这一步会列出远程所有分支每个分支都是红色字体形如 **origin/branch_name**
- git checkout **branch_name**：将除了 **main** 以外的所有 **branch_name** 都用这条指令操作一遍，就可以得到所有的分支

### 同步仓库更新

- git fetch：拉取所有分支的更新
- git pull：拉取当前分支的所有更新

### 上传更新至仓库

- git add .：提交至暂存区
- git commit -m "注释"：提交更改
- git push：推送至远程仓库

### 分支相关

- git branch：列出所有本地分支
- git branch -r：列出所有远程分支
- git checkout **branch_name**：切换至 **branch_name** 分支