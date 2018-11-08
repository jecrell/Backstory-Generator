using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Backstory_Generator
{
    public static class GridViewUtility
    {
        public static void DeleteRow<T>(DataGridView viewer, List<T> list, DataGridViewCellEventArgs e)
        {
            //if click is on new row or header row
            if (e.RowIndex == viewer.NewRowIndex || e.RowIndex < 0)
                return;
            
            //Check if click is on specific column 
            if (e.ColumnIndex == viewer.Columns["dataGridViewDeleteButton"].Index)
            {
                var temp = viewer.DataSource;
                viewer.DataSource = null;
                list.RemoveAt(e.RowIndex);
                GridViewUtility.UpdateView(viewer, (List<T>)temp);
            }
         }

        public static async void UpdateView<T>(DataGridView viewer, List<T> dataSource)
        {
            //Clears in-case of leftover data
            if (viewer?.Columns?.Count > 0)
                viewer?.Columns?.Clear();
            if (viewer?.Rows?.Count > 0)
                viewer?.Rows?.Clear();
            viewer.DataSource = dataSource;
            await Task.Delay(20);

            //Easy to use delete button helps users manage lists
            var deleteButton = new DataGridViewButtonColumn();
            deleteButton.Name = "dataGridViewDeleteButton";
            deleteButton.HeaderText = "Delete";
            deleteButton.Text = "X";
            deleteButton.UseColumnTextForButtonValue = true;
            if (viewer?.Columns?.Count > 1)
            {
                viewer.Columns.Add(deleteButton);
                viewer.Columns[0].Width = 75;
                viewer.Columns[1].Width = 30;
                viewer.Columns[2].Width = 25;
            }
        }

        public static void AddRow<T>(DataGridView viewer, List<T> row, T data, EventArgs e)
        {
            if (row == null)
                row = new List<T>();

            if (row.FirstOrDefault(x => x.Equals(data)) != null)
                return;
            row.Add(data);

            var temp = viewer.DataSource;
            viewer.DataSource = null;
            GridViewUtility.UpdateView(viewer, row);
        }
        
    }
}
