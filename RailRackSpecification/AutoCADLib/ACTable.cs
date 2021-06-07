using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace AutoCADLib
{
    class ACTable
    {
        Table table = new Table();
        Transaction tr;
        private int rowsNum; //Количество строк
        private int columnsNum;//Количество столбцов
        // Высота строки
        double rowHeight = 3;//Высота строки
        double columnWidth = 20;//Ширина столбца
        public ACTable() { }
        public ACTable(int rowsNum, int columnsNum, Transaction tr) {
            this.rowsNum = rowsNum;
            this.columnsNum = columnsNum;
            this.tr = tr;
        }
        public Table Table
        {
            get
            {
                return table;
            }
        }
        public void AddDevices()
        {
            for (int i = 0;
            i < rowsNum;
            i++)
            {
                for (int j = 0;
                    j < columnsNum;
                    j++)
                {
                    table.Cells[i, j].TextHeight = 1;
                    if (i == 0 && j == 0)
                        table.Cells[i, j].TextString =
                            "Заголовок";
                    else
                        table.Cells[i, j].TextString =
                            i.ToString() + "," + j.ToString();

                    table.Cells[i, j].Alignment =
                        CellAlignment.MiddleCenter;
                }
            }
        }
        public void CreateTable()
        {
            Database db =
        HostApplicationServices.WorkingDatabase;
            Document acDoc = Application.DocumentManager.MdiActiveDocument;


            
                BlockTable bt =
                    (BlockTable)tr.GetObject(db.BlockTableId,
                                            OpenMode.ForRead);
                ObjectId msId =
                    bt[BlockTableRecord.ModelSpace];

                BlockTableRecord btr =
                    (BlockTableRecord)tr.GetObject(msId,
                                        OpenMode.ForWrite);

                // create a table
                
                table.TableStyle = db.Tablestyle;

                

                

                // Добавляем строки и колонки
                table.InsertRows(0,
                            rowHeight,
                            rowsNum);
                table.InsertColumns(0,
                            columnWidth,
                            columnsNum);

                table.SetRowHeight(rowHeight);
                table.SetColumnWidth(columnWidth);

                Point3d eMax = db.Extmax;
                Point3d eMin = db.Extmin;
                double CenterY =
                    (eMax.Y + eMin.Y) * 0.5;

                PromptPointResult pPtRes;
                PromptPointOptions pPtOpts = new PromptPointOptions("");
               

                

                // Prompt for the end point
                pPtOpts.Message = "\nEnter the end point of the line: ";
                pPtOpts.UseBasePoint = true;
                
                pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                Point3d ptEnd = pPtRes.Value;

                table.Position = ptEnd;

                // заполняем по одной все ячейки
               
                
                table.GenerateLayout();
                btr.AppendEntity(table);
                tr.AddNewlyCreatedDBObject(table, true);
                tr.Commit();
            }
        }
 }
