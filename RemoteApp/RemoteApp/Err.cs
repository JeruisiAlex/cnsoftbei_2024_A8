﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKRO_rdp
{
    public class Err
    {
        private static Err err = new Err();
        public static Err getErr() { return err; }

        private ErrType errType;

        private string[] ErrContent = new string[7]
        {
            "",
            "获取应用列表或已发布应用列表失败",
            "不能发布一个已经发布的应用",
            "远程打开一个应用发现应用不存在",
            "该应用不能卸载",
            "路径下不存在该应用，应用可能已经迁移",
            "不能发布这个应用",
        };

        private Err()
        {
            errType = ErrType.SUCCESS;
        }

        public void handle()
        {
            ErrType errType = getErrType();
            if (errType != ErrType.SUCCESS)
            {
                MessageBox.Show(ErrContent[(int)errType]);
            }
        }
        public ErrType getErrType()
        {
            ErrType result = errType;
            errType = ErrType.SUCCESS;
            return result;
        }
        public void setErrType(ErrType errType) { this.errType = errType; }
    }
}
