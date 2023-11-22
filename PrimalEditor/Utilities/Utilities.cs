using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimalEditor.Utilities {
    public static class ID {
        public static int INVALID_ID => -1;                            //常量无效ID值
        public static bool IsValid(int id) => id != INVALID_ID;        //检查ID是否无效的方法
    }

    public static class  MathUtil{
        public static float Epsilon => 0.00001f;

        public static bool IsTheSameAs(this float value, float other) { 
            return Math.Abs(value - other) < Epsilon;
        }

        public static bool IsTheSameAs(this float? value, float? other) {
            if(!value.HasValue || !other.HasValue) return false;
            return Math.Abs(value.Value - other.Value) < Epsilon;
        }
    }
}
