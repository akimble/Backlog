using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Backlog
{
    class MyColors
    {
        private static Brush darkGrey = (Brush) (new BrushConverter().ConvertFromString("#FF272525"));
        private static Brush white = Brushes.White;
        private static Brush garnet = (Brush)(new BrushConverter().ConvertFromString("#bc010b"));
        private static Brush green = (Brush)(new BrushConverter().ConvertFromString("#FF24A836"));

        public static Brush DarkGrey()
        {
            return darkGrey;
        }

        public static Brush White()
        {
            return white;
        }

        public static Brush Garnet()
        {
            return garnet;
        }

        public static Brush Green()
        {
            return green;
        }
    }
}
