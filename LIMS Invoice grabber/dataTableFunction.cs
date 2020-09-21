using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS_Invoice_grabber {
    // datatable functionality
    class dataTableFunction {
        // sends table to 2d array
        public static object[,] tableToArray(DataTable table) {
            object[,] array = new object[table.Rows.Count, table.Columns.Count];
            for (int i = 0; i < table.Rows.Count; i++) {
                for (int j = 0; j < table.Columns.Count; j++) {
                    array[i, j] = table.Rows[i][j];
                }
            }
            return array;
        }
        // gets table headers
        public static object[] tableHeaders(DataTable table) {
            object[] array = new object[table.Columns.Count];
            for (int i = 0; i < table.Columns.Count; i++) {
                array[i] = table.Columns[i].ColumnName;
            }
            return array;
        }
    }
}
