﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnsoftbei_A8
{
    public class Err
    {
        private static Err err = new Err();
        public static Err getErr() { return err; }

        private ErrType errType;
        private Err() {
            errType = ErrType.SUCCESS;
        }

        public void handle()
        {

        }
        public ErrType getErrType() {
            ErrType result = errType;
            errType = ErrType.SUCCESS;
            return result; 
        }
        public void setErrType(ErrType errType) { this.errType = errType; }
    }
}
