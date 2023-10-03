using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskManager
{
    internal class ListViewItemComparer : IComparer
    {
        private int _columnIndex;

        public int ColumnIndex
        {
            get
            {
                return _columnIndex;
            }
            set
            {
                _columnIndex = value;
            }
        }

        private SortOrder _sortDirection;

        public SortOrder SortDirection
        {
            get
            {
                return _sortDirection;
            }
            set
            {
                _sortDirection = value;
            }
        }

        public ListViewItemComparer()
        {
            _sortDirection = SortOrder.None;
        }

        public int Compare(object x, object y)
        {
            ListViewItem listViewItemX = x as ListViewItem;
            ListViewItem listViewItemY = y as ListViewItem;

            int result;

            switch (_columnIndex)
            {
                case 0:
                    
                    result = string.Compare(listViewItemX.SubItems[_columnIndex].Text,
                        listViewItemY.SubItems[_columnIndex].Text, false);

                    break;

                case 1:

                    double valueX = double.Parse(listViewItemX.SubItems[_columnIndex].Text);
                    double valueY = double.Parse(listViewItemY.SubItems[_columnIndex].Text);

                    result = valueX.CompareTo(valueY);

                    break;

                default:

                    result = string.Compare(listViewItemX.SubItems[_columnIndex].Text,
                        listViewItemY.SubItems[_columnIndex].Text, false);

                    break;
            }

            if(_sortDirection == SortOrder.Descending)
            {
                return -result;
            }
            else
            {
                return result;
            }
        }
    }
}
