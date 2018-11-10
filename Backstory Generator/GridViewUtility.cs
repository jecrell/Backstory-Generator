using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Backstory_Generator
{
    public static class GridViewUtility
    {
        public static void DeleteRow<T>(DataGridView viewer, BindingList<T> list, DataGridViewCellEventArgs e)
        {
            //if click is on new row or header row
            if (e.RowIndex == viewer.NewRowIndex || e.RowIndex < 0)
                return;
            
            //Check if click is on specific column 
            if (e.ColumnIndex == viewer.Columns["dataGridViewDeleteButton"].Index)
            {
                list.RemoveAt(e.RowIndex);
            }
         }

        public static async void UpdateView<T>(DataGridView viewer, IEnumerable<T> dataSource, int[] widths = null, List<string> hiddenColumns = null)
        {
            //Clears in-case of leftover data
            if (viewer?.Columns?.Count > 0)
                viewer?.Columns?.Clear();
            if (viewer?.Rows?.Count > 0)
                viewer?.Rows?.Clear();
            viewer.DataSource = null;
            viewer.DataSource = dataSource;
            await Task.Delay(20);

            //Easy to use delete button helps users manage lists
            var deleteButton = new DataGridViewButtonColumn();
            deleteButton.Name = "dataGridViewDeleteButton";
            deleteButton.HeaderText = "Delete";
            deleteButton.Text = "X";
            deleteButton.UseColumnTextForButtonValue = true;
            
                if (hiddenColumns != null && hiddenColumns?.Count > 0)
                {
                    foreach (var col in hiddenColumns)
                    {
                        viewer.Columns.Remove(col);
                    }
                    viewer.Columns.Add(deleteButton);
                }

                if (widths != null)
                {
                    for (int i = 0; i < widths.Length; i++)
                        viewer.Columns[i].Width = widths[i];
                }
                else
                {
                    if (viewer.Columns.Count > 2)
                {

                    viewer.Columns[0].Width = 75;
                    viewer.Columns[1].Width = 30;
                    viewer.Columns[2].Width = 25;
                }
                    else if (viewer.Columns.Count > 1)
                {
                    viewer.Columns[0].Width = 75;
                    viewer.Columns[1].Width = 25;
                }
                else if (viewer.Columns.Count > 0)
                {

                    viewer.Columns[0].Width = 75;
                }
                }
            
        }


        public static void AddRow<T>(DataGridView viewer, BindingList<T> row, T data, EventArgs e)
        {
            if (row == null)
            {
                row = new BindingList<T>();
                viewer.DataSource = row;
            };

            if (row.FirstOrDefault(x => x.Equals(data)) != null)
                return;
            row.Add(data);
            
        }
        
    }
}
