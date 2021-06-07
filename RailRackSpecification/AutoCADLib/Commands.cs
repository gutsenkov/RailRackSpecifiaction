using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Geometry;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace AutoCADLib
{
    public class Commands : IExtensionApplication
    {
        // функция инициализации (выполняется при загрузке плагина)
        public void Initialize()
        {
            MessageBox.Show("AutoCADLib v0.1 beta");
        }
        // функция, выполняемая при выгрузке плагина
        public void Terminate()
        {
            MessageBox.Show("Goodbye!");
        }
        [CommandMethod ("SPECIFICATION")]
        public void MyCommand() {
            Database db =
        HostApplicationServices.WorkingDatabase;
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction()) {
                // Создание массива TypedValue для определение критериев фильтра
                TypedValue[] acTypValAr = new TypedValue[2];
                acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
                acTypValAr.SetValue(new TypedValue((int)DxfCode.LayerName, "Спецификация"), 1);
               
                 // Назначение критериев фильтра объекту SeectionFilter
                SelectionFilter acSelFtr = new SelectionFilter(acTypValAr);
                PromptSelectionResult acSSPrompt = acDoc.Editor.GetSelection(acSelFtr);
                if (acSSPrompt.Status == PromptStatus.OK) {
                    List<Device> devices = new List<Device>();
                    SelectionSet acSet = acSSPrompt.Value;
                    int colObject = 0;
                    foreach (SelectedObject acSSObj in acSet) {
                        MText text = acTrans.GetObject(acSSObj.ObjectId,OpenMode.ForRead) as MText;
                        
                        Device device = new Device(text.Text);
                        // Бинарный поиск
                       
                        int indexBin = devices.BinarySearch(device, new DeviceComparer());
                        if (indexBin < 0)
                        {
                            devices.Add(device);
                            colObject++;
                        }
                        else
                        {
                            devices[indexBin].AddDevice();
                        }
                    }
                    
                    BlockTable bt =
        (BlockTable)acTrans.GetObject(db.BlockTableId,
                                OpenMode.ForRead);
                    ObjectId msId =
                        bt[BlockTableRecord.ModelSpace];

                    BlockTableRecord btr =
                        (BlockTableRecord)acTrans.GetObject(msId,
                                            OpenMode.ForWrite);

                    // create a table
                    Table tb = new Table();
                    tb.TableStyle = db.Tablestyle;

                    // Число строк
                    Int32 RowsNum = colObject++;
                    // Число столбцов
                    Int32 ColumnsNum = 1;

                    // Высота строки
                    double rowheight = 5;
                    // Ширина столбца
                    double columnwidth = 50;

                    // Добавляем строки и колонки
                    tb.InsertRows(0,
                                rowheight,
                                RowsNum);
                    tb.InsertColumns(0,
                                columnwidth,
                                ColumnsNum);

                    tb.SetRowHeight(rowheight);
                    tb.SetColumnWidth(columnwidth);

                    Point3d eMax = db.Extmax;
                    Point3d eMin = db.Extmin;
                    double CenterY =
                        (eMax.Y + eMin.Y) * 0.5;
                    PromptPointResult pPtRes;
                    PromptPointOptions pPtOpts = new PromptPointOptions("");




                    // Prompt for the end point
                    pPtOpts.Message = "\nEnter the point: ";
                    pPtOpts.UseBasePoint = false;

                    pPtRes = acDoc.Editor.GetPoint(pPtOpts);
                    Point3d ptEnd = pPtRes.Value;

                    
                    tb.Position = ptEnd;

                    // заполняем по одной все ячейки
                    int i = 0;
                    foreach(Device dev in devices)
                    {
                            tb.Cells[i, 0].TextHeight = 3;
                            tb.Cells[i, 1].TextHeight = 3;
                            tb.Cells[i, 0].TextString = dev.Name;
                            tb.Cells[i, 1].TextString = dev.Quantity.ToString();
                            tb.Cells[i, 0].Alignment =CellAlignment.MiddleCenter;
                            tb.Cells[i, 1].Alignment = CellAlignment.MiddleCenter;
                        i++;
                           
                        
                    }

                    tb.GenerateLayout();
                    btr.AppendEntity(tb);
                    acTrans.AddNewlyCreatedDBObject(tb, true);
                    acTrans.Commit();
                }
            }
        } 
    }
}
