using Microsoft.Data.ConnectionUI;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

/*
C# .NET Framework 4.7 Console App to Read a no-Header Pipe-delimited file into SQL Server database table.
*/

namespace SQLtextFielReaderPipeDelimited
{
    partial class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            DataSource sqlDataSource = new DataSource("MicrosoftSqlServer", "Microsoft SQL Server");
            sqlDataSource.Providers.Add(DataProvider.SqlDataProvider);

			// we will use a SQL connection dialog so we do not store a connection string with sensitive credentials
			
            using (var connect = new DataConnectionDialog())
            using (var folder = new FolderBrowserDialog())
            {
                connect.DataSources.Add(sqlDataSource);
                connect.SelectedDataProvider = DataProvider.SqlDataProvider;
                connect.SelectedDataSource = sqlDataSource;
                connect.ConnectionString = Default.Connection;

                if (DataConnectionDialog.Show(connect) == DialogResult.OK)
                    using (var connection = new SqlConnection(connect.ConnectionString))
                    {
                        sw.Start();
						 
						/* replace the file path and file name below with your file or place the text file inside the  
						Bin\Debug folder to have the Visual Studio Solution find inherently */
						
                        var path = (@"C:\\Users\\bob\\source\\repo\\SampleFolder\\TextFilePipesNoHeaders_20200701.txt");

                        List<string> lines = File.ReadAllLines(path).ToList();

                        foreach (var line in lines)
                        {
                            // we are reading a pipe-delimited file but could replace the below with ',' instead
							string[] entries = line.Split('|');

                            using (SqlConnection conn = new SqlConnection(connect.ConnectionString))
                            {
                                connection.Open();

								/* replace 'SampleDataBaseName.dbo.SampleTableName' with your database and table, no quotes & 
								no square brackets */
								
                                string query = @"INSERT INTO SampleDataBaseName.dbo.SampleTableName VALUES (@Column0, @Column1 , @Column2 , @Column3 , @Column4, @Column5, @Column6, @Column7, @Column8)";

                                SqlCommand cmd = new SqlCommand(query, connection);

                                cmd.Parameters.AddWithValue("@Column0", entries[0]);
                                cmd.Parameters.AddWithValue("@Column1", entries[1]);
                                cmd.Parameters.AddWithValue("@Column2", entries[2]);
                                cmd.Parameters.AddWithValue("@Column3", entries[3]);
                                cmd.Parameters.AddWithValue("@Column4", entries[4]);
                                cmd.Parameters.AddWithValue("@Column5", entries[5]);
                                cmd.Parameters.AddWithValue("@Column6", entries[6]);
                                cmd.Parameters.AddWithValue("@Column7", entries[7]);
                                cmd.Parameters.AddWithValue("@Column8", entries[8]);

                                SqlDataReader dr = cmd.ExecuteReader();

                                connection.Close();
                            }
                        }
                        Console.WriteLine(Environment.NewLine + "Writing Data to database..." + Environment.NewLine);

                        sw.Stop();

                        Console.WriteLine("File contained " + lines.Count + " entries.");
                        Console.WriteLine("File took " + sw.Elapsed.TotalSeconds + " seconds to Process.");
                        Console.WriteLine("Press Enter to finish.");
                        Console.ReadLine();
                    }
            }
        }
    }
}





