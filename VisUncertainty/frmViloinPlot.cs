﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;


namespace VisUncertainty
{
    public partial class frmViolinPlot : Form
    {
        private MainForm m_pForm;
        private IActiveView m_pActiveView;

        private clsSnippet m_pSnippet;

        public frmViolinPlot()
        {
            try
            {
                InitializeComponent();
                m_pForm = System.Windows.Forms.Application.OpenForms["MainForm"] as MainForm;
                m_pActiveView = m_pForm.axMapControl1.ActiveView;

                for (int i = 0; i < m_pForm.axMapControl1.LayerCount; i++)
                {
                    cboTargetLayer.Items.Add(m_pForm.axMapControl1.get_Layer(i).Name);
                }
                m_pSnippet = new clsSnippet();
            }
            catch
            {
                MessageBox.Show("Error 221");
                return;
            }
        }

        private void cboTargetLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string strLayerName = cboTargetLayer.Text;

                int intLIndex = m_pSnippet.GetIndexNumberFromLayerName(m_pActiveView, strLayerName);
                ILayer pLayer = m_pForm.axMapControl1.get_Layer(intLIndex);

                IFeatureLayer pFLayer = pLayer as IFeatureLayer;
                ESRI.ArcGIS.Geodatabase.IFeatureClass pFClass = pFLayer.FeatureClass;

                IFields fields = pFClass.Fields;

                cboFieldName.Items.Clear();
                cboGroupField.Items.Clear();

                for (int i = 0; i < fields.FieldCount; i++)
                {
                    if (fields.get_Field(i).Type == esriFieldType.esriFieldTypeString)
                        cboGroupField.Items.Add(fields.get_Field(i).Name);
                    else
                        cboFieldName.Items.Add(fields.get_Field(i).Name);
                }
            }
            catch
            {
                MessageBox.Show("Error 222");
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBoxplot_Click(object sender, EventArgs e)
        {
            try
            {
                string strTargetLayerName = cboTargetLayer.Text;
                string strFieldName = cboFieldName.Text;
                string strGroupFieldName = cboGroupField.Text;

                int intTargetIndex = m_pSnippet.GetIndexNumberFromLayerName(m_pActiveView, strTargetLayerName);
                int intNFeatureCount = 0;

                ILayer pTargetLayer = m_pForm.axMapControl1.get_Layer(intTargetIndex);

                IFeatureLayer pTargetFLayer = (IFeatureLayer)pTargetLayer;

                //Extract geometry from selected feature
                IFeatureCursor pFCursor = null;
                pFCursor = pTargetFLayer.Search(null, true);
                intNFeatureCount = pTargetFLayer.FeatureClass.FeatureCount(null);

                int intFieldIdx = pTargetFLayer.FeatureClass.Fields.FindField(strFieldName);
                int intGroupIdx = pTargetFLayer.FeatureClass.Fields.FindField(strGroupFieldName);

                System.Collections.Generic.List<object> uvList = new System.Collections.Generic.List<object>();
                ICursor ipCursor = pFCursor as ICursor;
                IRow ipRow = ipCursor.NextRow();
                while (ipRow != null)
                {
                    object curValue = ipRow.get_Value(intGroupIdx);

                    if (!uvList.Contains(curValue))
                    {
                        uvList.Add(curValue);
                    }

                    ipRow = ipCursor.NextRow();
                }
                uvList.Sort();

                //Add exeption for sorting //0421 HK
                //1. H L M
                if (uvList.Count == 3)
                {
                    if (uvList[0].ToString() == "H" && uvList[1].ToString() == "L" && uvList[2].ToString() == "M")
                    {
                        uvList.Clear();
                        uvList.Add("L");
                        uvList.Add("M");
                        uvList.Add("H");
                    }
                    if (uvList[0].ToString() == "High" && uvList[1].ToString() == "Low" && uvList[2].ToString() == "Medium")
                    {
                        uvList.Clear();
                        uvList.Add("Low");
                        uvList.Add("Medium");
                        uvList.Add("High");

                    }
                }

                ipCursor.Flush();

                int intNGroups = uvList.Count;
                double[,] adbBPStats = new double[5, uvList.Count];
                double[,] adbQuantiles = new double[5, uvList.Count];

                IQueryFilter pQfilter = new QueryFilterClass();
                double[][] adblTarget = new double[intNGroups][];
                int[] aintNCounts = new int[intNGroups];
                int i = 0;

                foreach (string uvValue in uvList)
                {
                    pQfilter.WhereClause = strGroupFieldName + " = '" + uvValue + "'";
                    pFCursor = pTargetFLayer.Search(pQfilter, true);
                    IFeature pFeature = pFCursor.NextFeature();
                    intNFeatureCount = pTargetFLayer.FeatureClass.FeatureCount(pQfilter);
                    aintNCounts[i] = intNFeatureCount;
                    adblTarget[i] = new double[intNFeatureCount];

                    int j = 0;
                    while (pFeature != null)
                    {
                        adblTarget[i][j] = Convert.ToDouble(pFeature.get_Value(intFieldIdx));
                        j++;
                        pFeature = pFCursor.NextFeature();
                    }
                    pFCursor.Flush();

                    i++;
                }


                frmBoxPlotResults pfrmBoxplotResults = new frmBoxPlotResults();
                pfrmBoxplotResults.Text = "Violin plot of " + pTargetFLayer.Name;
                pfrmBoxplotResults.pChart.Series.Clear();
                ChartArea area = pfrmBoxplotResults.pChart.ChartAreas[0];
                int intMaxFeatureNCount = aintNCounts.Max();


                //Draw Lines
                for (int k = 0; k < intNGroups; k++)
                {
                    string strNumbering = k.ToString();
                    double[] adblStats = new double[5];
                    adblStats = BoxPlotStats(adblTarget[k]);
                    for (int l = 0; l < 5; l++)
                    {
                        adbQuantiles[l, k] = adblStats[l];
                    }


                    int intXvalue = k * 2 + 1;


                    double dblXmin = Convert.ToDouble(intXvalue) - 0.2;
                    double dblXmax = Convert.ToDouble(intXvalue) + 0.2;
                    double dblXBoxMin = Convert.ToDouble(intXvalue) - 0.1;
                    double dblXBoxMax = Convert.ToDouble(intXvalue) + 0.1;

                    //AddLineSeries(pfrmBoxplotResults, "min_" + strNumbering, Color.Black, 1, ChartDashStyle.Solid, dblXmin, dblXmax, adbQuantiles[0, k], adbQuantiles[0, k]);
                    AddLineSeries(pfrmBoxplotResults, "BoxBottom_" + strNumbering, Color.Black, 1, ChartDashStyle.Solid, dblXBoxMin, dblXBoxMax, adbQuantiles[1, k], adbQuantiles[1, k]);
                    AddLineSeries(pfrmBoxplotResults, "BoxLeft_" + strNumbering, Color.Black, 1, ChartDashStyle.Solid, dblXBoxMin, dblXBoxMin, adbQuantiles[1, k], adbQuantiles[3, k]);
                    AddLineSeries(pfrmBoxplotResults, "BoxRight_" + strNumbering, Color.Black, 1, ChartDashStyle.Solid, dblXBoxMax, dblXBoxMax, adbQuantiles[1, k], adbQuantiles[3, k]);
                    AddLineSeries(pfrmBoxplotResults, "Boxup_" + strNumbering, Color.Black, 1, ChartDashStyle.Solid, dblXBoxMin, dblXBoxMax, adbQuantiles[3, k], adbQuantiles[3, k]);
                    AddLineSeries(pfrmBoxplotResults, "median_" + strNumbering, Color.Red, 2, ChartDashStyle.Solid, dblXmin, dblXmax, adbQuantiles[2, k], adbQuantiles[2, k]);
                    //AddLineSeries(pfrmBoxplotResults, "max_" + strNumbering, Color.Black, 1, ChartDashStyle.Solid, dblXmin, dblXmax, adbQuantiles[4, k], adbQuantiles[4, k]);
                    //AddLineSeries(pfrmBoxplotResults, "verDown_" + strNumbering, Color.Black, 1, ChartDashStyle.Dash, intXvalue, intXvalue, adbQuantiles[0, k], adbQuantiles[1, k]);
                    //AddLineSeries(pfrmBoxplotResults, "verUp_" + strNumbering, Color.Black, 1, ChartDashStyle.Dash, intXvalue, intXvalue, adbQuantiles[3, k], adbQuantiles[4, k]);
                    AddLineSeries(pfrmBoxplotResults, "ver_" + strNumbering, Color.Black, 1, ChartDashStyle.Dash, intXvalue, intXvalue, adbQuantiles[0, k], adbQuantiles[4, k]);


                    //Draw Violin Plot
                    var pviolin = new System.Windows.Forms.DataVisualization.Charting.Series
                    {
                        Name = "vio1_" + strNumbering,
                        Color = Color.Black,
                        BorderColor = Color.Black,
                        IsVisibleInLegend = false,
                        ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line

                    };
                    pfrmBoxplotResults.pChart.Series.Add(pviolin);

                    double dblBandwidth = RuleofThumb(adblTarget[k]);
                    double[,] adblViolinStats = ViolinPlot(adblTarget[k], dblBandwidth);


                    int intChartLenth = (adblViolinStats.Length) / 2;

                    pviolin.Points.AddXY(intXvalue, adblViolinStats[0, 0]);

                    for (int l = 0; l < intChartLenth; l++)
                        pviolin.Points.AddXY(intXvalue - adblViolinStats[l, 1], adblViolinStats[l, 0]);

                    pviolin.Points.AddXY(intXvalue, adblViolinStats[intChartLenth - 1, 0]);

                    for (int l = intChartLenth - 1; l >= 0; l--)
                        pviolin.Points.AddXY(intXvalue + adblViolinStats[l, 1], adblViolinStats[l, 0]);

                    pviolin.Points.AddXY(intXvalue, adblViolinStats[0, 0]);

                    //for (int l = 0; l < intChartLenth; l++)
                    //{
                    //    if (adblViolinStats[l, 0] > adblStats[0] && adblViolinStats[l, 0] < adblStats[4])
                    //        pviolin.Points.AddXY(intXvalue - adblViolinStats[l, 1], adblViolinStats[l, 0]);
                    //}

                    //if (adblViolinStats[intChartLenth - 1, 0] < adblStats[4])
                    //    pviolin.Points.AddXY(intXvalue, adblStats[4]);
                    //else
                    //    pviolin.Points.AddXY(intXvalue, adblStats[4]);


                    //for (int l = intChartLenth - 1; l >= 0; l--)
                    //    if (adblViolinStats[l, 0] > adblStats[0] && adblViolinStats[l, 0] < adblStats[4])
                    //        pviolin.Points.AddXY(intXvalue + adblViolinStats[l, 1], adblViolinStats[l, 0]);

                    //if (adblViolinStats[0, 0] > adblStats[0])
                    //    pviolin.Points.AddXY(intXvalue, adblViolinStats[0, 0]);
                    //else
                    //    pviolin.Points.AddXY(intXvalue, adblStats[0]);


                }

                //Add Points
                System.Drawing.Color pMarkerColor = System.Drawing.Color.Blue;
                var seriesOutPts = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "Points",
                    Color = pMarkerColor,
                    BorderColor = pMarkerColor,
                    IsVisibleInLegend = false,
                    IsXValueIndexed = false,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point,
                    MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle

                };

                pfrmBoxplotResults.pChart.Series.Add(seriesOutPts);

                for (int k = 0; k < intNGroups; k++)
                {
                    int intXvalue = k * 2 + 1;
                    foreach (double dblValue in adblTarget[k])
                    {
                        //if (dblValue < adbQuantiles[1, k] || dblValue > adbQuantiles[3, k])
                        seriesOutPts.Points.AddXY(intXvalue, dblValue);
                    }
                }

                pMarkerColor = System.Drawing.Color.Black;
                var seriesInPts = new System.Windows.Forms.DataVisualization.Charting.Series
                {
                    Name = "InPoints",
                    Color = pMarkerColor,
                    BorderColor = pMarkerColor,
                    IsVisibleInLegend = false,
                    IsXValueIndexed = false,
                    ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point,
                    MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle

                };

                pfrmBoxplotResults.pChart.Series.Add(seriesInPts);

                for (int k = 0; k < intNGroups; k++)
                {
                    int intXvalue = k * 2 + 1;
                    foreach (double dblValue in adblTarget[k])
                    {
                        if (dblValue >= adbQuantiles[0, k] && dblValue <= adbQuantiles[4, k])
                            seriesInPts.Points.AddXY(intXvalue, dblValue);
                    }
                }


                //Store variables in frmBoxPlotResults
                pfrmBoxplotResults.mForm = m_pForm;
                pfrmBoxplotResults.pActiveView = m_pActiveView;
                pfrmBoxplotResults.pFLayer = pTargetFLayer;
                pfrmBoxplotResults.strFieldName = strFieldName;
                pfrmBoxplotResults.strGroupFieldName = strGroupFieldName;
                pfrmBoxplotResults.uvList = uvList;
                //pfrmBoxplotResults.pFillColor = pFillColor;
                pfrmBoxplotResults.adbQuantiles = adbQuantiles;
                pfrmBoxplotResults.pChart.ChartAreas[0].AxisX.Title = strGroupFieldName;
                pfrmBoxplotResults.pChart.ChartAreas[0].AxisY.Title = strFieldName;
                pfrmBoxplotResults.pChart.ChartAreas[0].AxisY.IsStartedFromZero = false;
                pfrmBoxplotResults.pChart.ChartAreas[0].AxisX.Maximum = intNGroups * 2;
                pfrmBoxplotResults.pChart.ChartAreas[0].AxisX.Interval = 2;
                pfrmBoxplotResults.pChart.ChartAreas[0].AxisX.Minimum = 0;
                pfrmBoxplotResults.pChart.ChartAreas[0].AxisX.IntervalOffset = -1;
                pfrmBoxplotResults.pChart.ChartAreas[0].AxisX.IsLabelAutoFit = false;
                for (int n = 0; n < uvList.Count; n++)
                {
                    int intXvalue = n * 2 + 1;

                    double dblXmin = Convert.ToDouble(intXvalue) - 0.6;
                    double dblXmax = Convert.ToDouble(intXvalue) + 0.6;

                    CustomLabel pcutsomLabel = new CustomLabel();
                    pcutsomLabel.FromPosition = dblXmin;
                    pcutsomLabel.ToPosition = dblXmax;
                    pcutsomLabel.Text = uvList[n].ToString();

                    pfrmBoxplotResults.pChart.ChartAreas[0].AxisX.CustomLabels.Add(pcutsomLabel);
                }

                pfrmBoxplotResults.Show();
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog();pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                return;
            }
        }


        private double[] BoxPlotStats(double[] adblTarget)
        {
            double[] adblStats = new double[5];
            double[] BPStats = new double[5];
            System.Array.Sort(adblTarget);
            //adblStats[0] = adblTarget.Min();
            //adblStats[4] = adblTarget.Max();
            int intLength = adblTarget.Length;

            adblStats[2] = GetMedian(adblTarget);
            //Get 1st and 3rd Quantile
            if (intLength % 2 == 0)
            {
                int newLength = intLength / 2;
                double[] lowSubset = new double[newLength];
                double[] upperSubset = new double[newLength];
                System.Array.Copy(adblTarget, 0, lowSubset, 0, newLength);
                System.Array.Copy(adblTarget, newLength, upperSubset, 0, newLength);

                adblStats[1] = GetMedian(lowSubset);
                adblStats[3] = GetMedian(upperSubset);
            }
            else
            {
                int newLength = (intLength - 1) / 2;
                double[] lowSubset = new double[newLength];
                double[] upperSubset = new double[newLength];
                System.Array.Copy(adblTarget, 0, lowSubset, 0, newLength);
                System.Array.Copy(adblTarget, newLength + 1, upperSubset, 0, newLength);

                adblStats[1] = GetMedian(lowSubset);
                adblStats[3] = GetMedian(upperSubset);
            }
            double dblIQR = adblStats[3] - adblStats[1];
            adblStats[0] = adblStats[1] - (1.5 * dblIQR);
            adblStats[4] = adblStats[3] + (1.5 * dblIQR);
            return adblStats;
        }
        private double GetMedian(double[] sortedArray)
        {
            double dblMedian = 0;
            int intLength = sortedArray.Length;

            if (intLength % 2 == 0)
            {
                // count is even, average two middle elements
                double a = sortedArray[intLength / 2 - 1];
                double b = sortedArray[intLength / 2];
                return dblMedian = (a + b) / 2;
            }
            else
            {
                // count is odd, return the middle element
                return dblMedian = sortedArray[(intLength - 1) / 2];
            }
        }
        private double[,] ViolinPlot(double[] pdblTarget, double dblBandwidth)
        {
            System.Array.Sort(pdblTarget);
            int intLength = pdblTarget.Length;
            double dblMax = pdblTarget[intLength - 1];
            double dblMin = pdblTarget[0];
            double dblRange = dblMax - dblMin;
            
            //Fixed Counts on Y-axis as 50;
            double dblIncrement = dblRange / 50;
            int intCnt = 51;


            double[,] adblResult = new double[intCnt, 2];

            for (int i = 0; i < intCnt; i++)
            {
                double dblX = dblMin + (i * dblIncrement);
                adblResult[i, 0] = dblX;

                int j = 0;
                double dblU = (dblX - pdblTarget[j]) / dblBandwidth;
                double dblK = 0;

                while (dblU > -1) //Apply While to improve efficienty, it can apply only sorted lst or arry
                {
                    //Apply Epanechnikov
                    if (dblU < 1)
                    {
                        dblK += 0.75 * (1 - Math.Pow(dblU, 2));
                    }
                    j++;
                    if (j < intLength)
                        dblU = (dblX - pdblTarget[j]) / dblBandwidth;
                    else
                        dblU = -1;
                }
                //adblResult[i, 1] = dblK / (dblBandwidth * intLength);
                //adblResult[i, 1] = dblK;
                adblResult[i, 1] = dblK / intLength;
            }

            return adblResult;

        }
        private double RuleofThumb(double[] pdblTarget)
        {

            int intLength = pdblTarget.Length;
            double average = pdblTarget.Average();
            double sumOfSquaresOfDifferences = pdblTarget.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / intLength);

            double dblBandwidth = 2.35 * sd * Math.Pow(intLength, -0.2);
            return dblBandwidth;
            

        }
        private void AddLineSeries(frmBoxPlotResults pfrmBoxplotResults, string strSeriesName, System.Drawing.Color FillColor, int intWidth, ChartDashStyle BorderDash, double dblXMin, double dblXMax, double dblYMin, double dblYMax)
        {
            try
            {
            var pSeries = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = strSeriesName,
                Color = FillColor,
                //BorderColor = Color.Black,
                BorderWidth = intWidth,
                IsVisibleInLegend = false,
                IsXValueIndexed = false,
                BorderDashStyle = BorderDash,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line,

            };

            pfrmBoxplotResults.pChart.Series.Add(pSeries);

            pSeries.Points.AddXY(dblXMin, dblYMin);
            pSeries.Points.AddXY(dblXMax, dblYMax);
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog();pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                return;
            }

        }
    }
}