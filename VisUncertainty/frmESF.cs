﻿using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using RDotNet;
using ESRI.ArcGIS.Geometry;

//using Accord.Math;
//MASS, spdep, and maptools packages in R are required

namespace VisUncertainty
{
    public partial class frmESF : Form
    {
        private MainForm m_pForm;
        private IActiveView m_pActiveView;
        private clsSnippet m_pSnippet;
        private IFeatureLayer m_pFLayer;
        private IFeatureClass m_pFClass;
        private REngine m_pEngine;
        //Varaibles for SWM
        private bool m_blnCreateSWM = false;

        public frmESF()
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

                m_pEngine = m_pForm.pEngine;
                m_pEngine.Evaluate("library(spdep); library(maptools); library(MASS)");
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog();pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                return;
            }
        }

        private void cboTargetLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTargetLayer.Text != "" && cboFamily.Text != "")
                {

                    string strLayerName = cboTargetLayer.Text;

                    int intLIndex = m_pSnippet.GetIndexNumberFromLayerName(m_pActiveView, strLayerName);
                    ILayer pLayer = m_pForm.axMapControl1.get_Layer(intLIndex);


                    m_pFLayer = pLayer as IFeatureLayer;
                    m_pFClass = m_pFLayer.FeatureClass;

                    //New Spatial Weight matrix function 081017
                    clsSnippet.SpatialWeightMatrixType pSWMType = new clsSnippet.SpatialWeightMatrixType();
                    if (m_pFClass.ShapeType == esriGeometryType.esriGeometryPolygon) //Apply Different Spatial weight matrix according to geometry type 07052017 HK
                        txtSWM.Text = pSWMType.strPolySWM;
                    else if (m_pFClass.ShapeType == esriGeometryType.esriGeometryPoint)
                        txtSWM.Text = pSWMType.strPointSWM;
                    else if (m_pFClass.ShapeType == esriGeometryType.esriGeometryPolyline)
                        MessageBox.Show("Spatial weight matrix for polyline is not supported.");

                    IFields fields = m_pFClass.Fields;

                    cboFieldName.Items.Clear();
                    cboFieldName.Text = "";
                    lstFields.Items.Clear();
                    lstIndeVar.Items.Clear();
                    cboNormalization.Text = "";

                    if (cboFamily.Text == "Poisson" || cboFamily.Text == "Logistic" || cboFamily.Text == "Negative Binomial")
                    {
                        for (int i = 0; i < fields.FieldCount; i++)
                        {
                            if (m_pSnippet.FindNumberFieldType(fields.get_Field(i)))
                            {
                                lstFields.Items.Add(fields.get_Field(i).Name);
                                cboNormalization.Items.Add(fields.get_Field(i).Name);
                                if (fields.get_Field(i).Type == esriFieldType.esriFieldTypeInteger || fields.get_Field(i).Type == esriFieldType.esriFieldTypeSmallInteger)
                                    cboFieldName.Items.Add(fields.get_Field(i).Name);
                            }

                        }
                    }
                    else
                    {
                        for (int i = 0; i < fields.FieldCount; i++)
                        {
                            if (m_pSnippet.FindNumberFieldType(fields.get_Field(i)))
                            {
                                cboFieldName.Items.Add(fields.get_Field(i).Name);
                                lstFields.Items.Add(fields.get_Field(i).Name);
                                cboNormalization.Items.Add(fields.get_Field(i).Name);
                            }
                        }
                    }
                    //Add intercept in the listview for independent variables
                    lstIndeVar.Items.Add("Intercept");

                    if (chkSave.Checked)
                        UpdateListview(lstSave, m_pFClass);

                }
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog();pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                return;
            }
        }



        private void btnMoveRight_Click(object sender, EventArgs e)
        {
            m_pSnippet.MoveSelectedItemsinListBoxtoOtherListBox(lstFields, lstIndeVar);
        }

        private void btnMoveLeft_Click(object sender, EventArgs e)
        {
            m_pSnippet.MoveSelectedItemsinListBoxtoOtherListBox(lstIndeVar, lstFields);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            try
            {

                if (cboFieldName.Text == "")
                {
                    MessageBox.Show("Please select the dependent input variables to be used in the regression model.",
                        "Please choose at least one input variable");
                    return;
                }
                if (lstIndeVar.Items.Count == 0)
                {
                    MessageBox.Show("Please select independents input variables to be used in the regression model.",
                        "Please choose at least one input variable");
                    return;
                }
                //if (cboFamily.Text == "Binomial")
                //{
                //    MessageBox.Show("Please select a variable for normailization");
                //    return;
                //}
                frmProgress pfrmProgress = new frmProgress();
                pfrmProgress.lblStatus.Text = "Pre-Processing:";
                pfrmProgress.pgbProgress.Style = ProgressBarStyle.Marquee;
                pfrmProgress.Show();

                
                //Decimal places
                int intDeciPlaces = 5;

                //Get number of Independent variables 
                int nIndevarlistCnt = lstIndeVar.Items.Count;
                //Indicate an intercept only model (2) or a non-intercept model (1) or not (0)
                int intInterceptModel = 1;
                for (int j = 0; j < nIndevarlistCnt; j++)
                {
                    if ((string)lstIndeVar.Items[j] == "Intercept")
                        intInterceptModel = 0;
                }
                if (nIndevarlistCnt == 1 && intInterceptModel == 0)
                    intInterceptModel = 2;

                int nIDepen = 0;
                if (intInterceptModel == 0)
                    nIDepen = nIndevarlistCnt - 1;
                else if (intInterceptModel == 1)
                    nIDepen = nIndevarlistCnt;

                // Gets the column of the dependent variable
                String dependentName = (string)cboFieldName.SelectedItem;
                string strNoramlName = cboNormalization.Text;
                
                // Gets the columns of the independent variables
                String[] independentNames = new string[nIDepen];
                int intIdices = 0;
                string strIndependentName = "";
                for (int j = 0; j < nIndevarlistCnt; j++)
                {
                    strIndependentName = (string)lstIndeVar.Items[j];
                    if (strIndependentName != "Intercept")
                    {
                        independentNames[intIdices] = strIndependentName;
                        intIdices++;
                    }
                }

                int nFeature = m_pFClass.FeatureCount(null);

                //Warning for method
                if (nFeature > m_pForm.intWarningCount)
                {
                    DialogResult dialogResult = MessageBox.Show("It might take a lot of time. Do you want to continue?", "Warning", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.No)
                    {
                        return;
                    }
                }

                IFeatureCursor pFCursor = m_pFLayer.Search(null, true);
                IFeature pFeature = pFCursor.NextFeature();

                //Get index for independent and dependent variables
                int intDepenIdx = m_pFClass.Fields.FindField(dependentName);
                int intNoramIdx = -1;
                if (strNoramlName != "")
                    intNoramIdx = m_pFClass.Fields.FindField(strNoramlName);

                int[] idxes = new int[nIDepen];

                for (int j = 0; j < nIDepen; j++)
                {
                    idxes[j] = m_pFLayer.FeatureClass.Fields.FindField(independentNames[j]);
                }


                //Store independent values at Array
                double[] arrDepen = new double[nFeature];
                 double[][] arrInDepen = new double[nIDepen][];
                 double[] arrNormal = new double[nFeature];

                 for (int j = 0; j < nIDepen; j++)
                 {
                     arrInDepen[j] = new double[nFeature];
                 }

                int i = 0;
                while (pFeature != null)
                {

                    arrDepen[i] = Convert.ToDouble(pFeature.get_Value(intDepenIdx));

                    if (intNoramIdx != -1)
                        arrNormal[i] = Convert.ToDouble(pFeature.get_Value(intNoramIdx));

                    for (int j = 0; j < nIDepen; j++)
                    {
                        arrInDepen[j][i] =Convert.ToDouble(pFeature.get_Value(idxes[j]));
                    }

                    i++;
                    pFeature = pFCursor.NextFeature();
                }

                if (cboFamily.Text == "Binomial" && intNoramIdx == -1)
                {
                    double dblMaxDepen = arrDepen.Max();
                    double dblMinDepen = arrDepen.Min();
                    if(dblMinDepen < 0 || dblMaxDepen > 1)
                    {
                        MessageBox.Show("The value range of a dependent variable must be between 0 and 1");

                        pfrmProgress.Close();
                        return;
                    }

                }
                //Loading the stepwise function
                //string strStartPath = m_pForm.strPath;
                //string pathr = strStartPath.Replace(@"\", @"/");
                //m_pEngine.Evaluate("source('" + pathr + "/AllFunctions.R')");

                //Plot command for R
                StringBuilder plotCommmand = new StringBuilder();

                if (!m_blnCreateSWM)
                {
                    //Get the file path and name to create spatial weight matrix
                    string strNameR = m_pSnippet.FilePathinRfromLayer(m_pFLayer);

                    if (strNameR == null)
                        return;

                    //Create spatial weight matrix in R
                    if (m_pFClass.ShapeType == esriGeometryType.esriGeometryPolygon)
                        m_pEngine.Evaluate("sample.shp <- readShapePoly('" + strNameR + "')");
                    else if (m_pFClass.ShapeType == esriGeometryType.esriGeometryPoint)
                        m_pEngine.Evaluate("sample.shp <- readShapePoints('" + strNameR + "')");
                    else
                    {
                        MessageBox.Show("This geometry type is not supported");
                        pfrmProgress.Close();
                        this.Close();
                    }


                    int intSuccess = m_pSnippet.CreateSpatialWeightMatrix(m_pEngine, m_pFClass, txtSWM.Text, pfrmProgress);
                    if (intSuccess == 0)
                        return;
                }

                
                //Dependent variable to R vector
                NumericVector vecDepen = m_pEngine.CreateNumericVector(arrDepen);
                m_pEngine.SetSymbol(dependentName, vecDepen);
                //plotCommmand.Append("lm.full <- " + dependentName + "~");
                NumericVector vecNormal = null;
                if (cboFamily.Text == "Binomial" && intNoramIdx != -1)
                {
                    vecNormal = m_pEngine.CreateNumericVector(arrNormal);
                    m_pEngine.SetSymbol(strNoramlName, vecNormal);
                    plotCommmand.Append("cbind(" + dependentName + ", " + strNoramlName + "-" + dependentName + ")~");
                }
                else if (cboFamily.Text == "Poisson" && intNoramIdx != -1)
                {
                    vecNormal = m_pEngine.CreateNumericVector(arrNormal);
                    m_pEngine.SetSymbol(strNoramlName, vecNormal);
                    plotCommmand.Append(dependentName + "~");
                }
                else if (cboFamily.Text == "Negative Binomial" && intNoramIdx != -1)
                {
                    vecNormal = m_pEngine.CreateNumericVector(arrNormal);
                    m_pEngine.SetSymbol(strNoramlName, vecNormal);
                    plotCommmand.Append(dependentName + "~");
                }
                else
                    plotCommmand.Append(dependentName + "~");

                
                if(intInterceptModel == 2)
                {
                    plotCommmand.Append("1");
                }
                else
                {
                    for (int j = 0; j < nIDepen; j++)
                    {
                        NumericVector vecIndepen = m_pEngine.CreateNumericVector(arrInDepen[j]);
                        m_pEngine.SetSymbol(independentNames[j], vecIndepen);
                        plotCommmand.Append(independentNames[j] + "+");
                    }
                    plotCommmand.Remove(plotCommmand.Length - 1, 1);

                    if (intInterceptModel == 1)
                        plotCommmand.Append("-1");
                }

                m_pEngine.Evaluate("sample.n <- length(sample.nb)");
                m_pEngine.Evaluate("B <- listw2mat(sample.listb); M <- diag(sample.n) - matrix(1/sample.n, sample.n, sample.n); MBM <- M%*%B%*%M");
                m_pEngine.Evaluate("eig <- eigen(MBM)");
                m_pEngine.Evaluate("EV <- as.data.frame( eig$vectors[,]); colnames(EV) <- paste('EV', 1:NCOL(EV), sep='')");
                double dblNCandidateEvs = 0;

                pfrmProgress.lblStatus.Text = "Selecting Candidate EVs:";
                if (rbEquation.Checked)
                {
                    m_pEngine.Evaluate("np <- length(eig$values[eig$values>0])");
                    m_pEngine.Evaluate("zmc <- lm.morantest(lm(" + plotCommmand.ToString() + "), listw=sample.listw, zero.policy=TRUE)$statistic");
                    m_pEngine.Evaluate("p <- 1/(1+exp(2.1480-6.1808*(zmc+0.6)^0.1742/np^0.1298+3.3534/(zmc+0.6)^0.1742))");
                    m_pEngine.Evaluate("no.ev <- round(np*p,0); EV <- EV[,1:no.ev]");
                    dblNCandidateEvs = m_pEngine.Evaluate("no.ev").AsNumeric().First();
                }
                else if (rbEValue.Checked)
                {
                    string strEValue = nudEValue.Value.ToString();
                    string strDirection = cboDirection.Text;

                    if (strDirection == "Positive Only")
                    {
                        m_pEngine.Evaluate("np <- length(eig$values[eig$values/eig$values[1]>" + strEValue + "])");
                        m_pEngine.Evaluate("EV <- EV[,1:np]");
                        dblNCandidateEvs = m_pEngine.Evaluate("np").AsNumeric().First();
                    }
                    else if (strDirection == "Negative Only")
                    {
                        m_pEngine.Evaluate("n.all <- length(eig$values)");
                        m_pEngine.Evaluate("nn <- length(eig$values[eig$values/eig$values[sample.n] > " + strEValue + "])");
                        m_pEngine.Evaluate("n.start <- n.all-nn+1");
                        m_pEngine.Evaluate("EV <- EV[,n.start:n.all]");
                        dblNCandidateEvs = m_pEngine.Evaluate("nn").AsNumeric().First();
                    }
                    else if (strDirection == "Both")
                    {
                        m_pEngine.Evaluate("np <- length(eig$values[eig$values/eig$values[1]>" + strEValue + "])");
                        m_pEngine.Evaluate("n.all <- length(eig$values)");
                        m_pEngine.Evaluate("nn <- length(eig$values[eig$values/eig$values[sample.n] > " + strEValue + "])");
                        m_pEngine.Evaluate("n.start <- n.all-nn+1");
                        m_pEngine.Evaluate("EV <- EV[,c(1:np, n.start:n.all)]");
                        dblNCandidateEvs = m_pEngine.Evaluate("nn+np").AsNumeric().First();
                    }
                }
                if (cboFamily.Text == "Linear (Gaussian)")
                    LinearESF(pfrmProgress, m_pFLayer, plotCommmand.ToString(), nIDepen, independentNames, dblNCandidateEvs, intDeciPlaces, intInterceptModel);
                else if(cboFamily.Text == "Poisson")
                    PoissonESF(pfrmProgress, m_pFLayer, plotCommmand.ToString(), nIDepen, independentNames,  strNoramlName, dblNCandidateEvs, intDeciPlaces, intInterceptModel);
                else if (cboFamily.Text == "Negative Binomial")
                    NBESF(pfrmProgress, m_pFLayer, plotCommmand.ToString(), nIDepen, independentNames, strNoramlName, dblNCandidateEvs, intDeciPlaces, intInterceptModel);
                else if (cboFamily.Text == "Binomial" ||cboFamily.Text=="Logistic")
                    BinomESF(pfrmProgress, m_pFLayer, plotCommmand.ToString(), nIDepen, independentNames, dblNCandidateEvs, intDeciPlaces, intInterceptModel);

                pfrmProgress.Close();
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog(); pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                return;
            }
        }

        private void NBESF(frmProgress pfrmProgress, IFeatureLayer pFLayer, string strLM, int nIDepen, String[] independentNames, string strNoramlName, double dblNCandidateEvs, int intDeciPlaces, int intInterceptModel)
        {
            try
            {

                pfrmProgress.lblStatus.Text = "Selecting EVs";
                if (strNoramlName == "")
                {
                    m_pEngine.Evaluate("esf.full <- glm.nb(" + strLM + "+., data=EV)");
                    m_pEngine.Evaluate("esf.org <- glm.nb(" + strLM + ", data=EV)");
                }
                else
                {
                    try
                    {
                        m_pEngine.Evaluate("esf.full <- glm.nb(" + strLM + "+ offset(" + strNoramlName + ")+., data=EV)");
                        m_pEngine.Evaluate("esf.org <- glm.nb(" + strLM + "+ offset(" + strNoramlName + "), data=EV)");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An offset requires a logarithm form. Please check the model again.");
                        pfrmProgress.Close();
                        return;
                    }

                }
                //m_pEngine.Evaluate("sample.esf <- stepAIC(esf.org, scope=list(upper= esf.full), direction='forward')");
                //m_pEngine.Evaluate("sample.esf <- stepwise(esf.full, esf.org, verbose=FALSE)");
                m_pEngine.Evaluate("sample.esf <- stepAIC(esf.org, scope=list(lower = esf.org, upper= esf.full), verbose = FALSE)");
                pfrmProgress.lblStatus.Text = "Printing Output:";
                m_pEngine.Evaluate("sum.esf <- summary(sample.esf)");
                //m_pEngine.Evaluate("sample.lm <- lm(" + strLM + ")");

                NumericMatrix matCoe = m_pEngine.Evaluate("as.matrix(sum.esf$coefficient)").AsNumericMatrix();
                CharacterVector vecNames = m_pEngine.Evaluate("attributes(sum.esf$coefficients)$dimnames[[1]]").AsCharacter();

                double dblNullDevi = m_pEngine.Evaluate("sum.esf$null.deviance").AsNumeric().First();
                double dblNullDF = m_pEngine.Evaluate("sum.esf$df.null").AsNumeric().First();
                double dblResiDevi = m_pEngine.Evaluate("sum.esf$deviance").AsNumeric().First();
                double dblResiDF = m_pEngine.Evaluate("sum.esf$df.residual").AsNumeric().First();

                //MC Using Pearson residual (Lin and Zhang 2007, GA) 
                if (cboAlternative.Text == "Greater")
                    m_pEngine.Evaluate("sampleresi.mc <-moran.mc(residuals(sample.esf, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Less")
                    m_pEngine.Evaluate("sampleresi.mc <-moran.mc(residuals(sample.esf, type='pearson'), listw =sample.listb, nsim=999, alternative = 'less', zero.policy=TRUE)");
                else
                    m_pEngine.Evaluate("sampleresi.mc <-moran.mc(residuals(sample.esf, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");

                double dblResiMC = m_pEngine.Evaluate("sampleresi.mc$statistic").AsNumeric().First();
                double dblResiMCpVal = m_pEngine.Evaluate("sampleresi.mc$p.value").AsNumeric().First();

                if (cboAlternative.Text == "Greater")
                    m_pEngine.Evaluate("orgresi.mc <-moran.mc(residuals(esf.org, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Less")
                    m_pEngine.Evaluate("orgresi.mc <-moran.mc(residuals(esf.org, type='pearson'), listw =sample.listb, nsim=999, alternative = 'less', zero.policy=TRUE)");
                else
                    m_pEngine.Evaluate("orgresi.mc <-moran.mc(residuals(esf.org, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");

                double dblResiLMMC = m_pEngine.Evaluate("orgresi.mc$statistic").AsNumeric().First();
                double dblResiLMpVal = m_pEngine.Evaluate("orgresi.mc$p.value").AsNumeric().First();
                //Nagelkerke r squared
                //double dblPseudoRsquared = m_pEngine.Evaluate("(1 - exp((sample.esf$deviance - sample.esf$null.deviance)/sample.n))/(1 - exp(-sample.esf$null.deviance/sample.n))").AsNumeric().First();
                double dblPseudoRsquared = m_pEngine.Evaluate("summary(lm(sample.esf$y~sample.esf$fitted.values))$r.squared").AsNumeric().First();

                NumericVector nvecNonAIC = m_pEngine.Evaluate("sample.esf$aic").AsNumeric();

                //Open Ouput form
                frmESFResult pfrmRegResult = new frmESFResult();
                if (strNoramlName == "")
                    pfrmRegResult.Text = "ESF Negative Binomial Regression Summary";
                else
                    pfrmRegResult.Text = "ESF Negative Binomial Regression with Offset (" + strNoramlName + ") Summary";

                //Create DataTable to store Result
                System.Data.DataTable tblRegResult = new DataTable("ESFResult");

                //Assign DataTable
                DataColumn dColName = new DataColumn();
                dColName.DataType = System.Type.GetType("System.String");
                dColName.ColumnName = "Name";
                tblRegResult.Columns.Add(dColName);

                DataColumn dColValue = new DataColumn();
                dColValue.DataType = System.Type.GetType("System.Double");
                dColValue.ColumnName = "Estimate";
                tblRegResult.Columns.Add(dColValue);

                DataColumn dColSE = new DataColumn();
                dColSE.DataType = System.Type.GetType("System.Double");
                dColSE.ColumnName = "Std. Error";
                tblRegResult.Columns.Add(dColSE);

                DataColumn dColTValue = new DataColumn();
                dColTValue.DataType = System.Type.GetType("System.Double");
                dColTValue.ColumnName = "z value";
                tblRegResult.Columns.Add(dColTValue);

                DataColumn dColPvT = new DataColumn();
                dColPvT.DataType = System.Type.GetType("System.Double");
                dColPvT.ColumnName = "Pr(>|z|)";
                tblRegResult.Columns.Add(dColPvT);

                int intNCoeff = 0;
                int intNSelectedEVs = matCoe.RowCount - nIDepen - 1;
                if (intInterceptModel == 1)
                    intNSelectedEVs = matCoe.RowCount - nIDepen;

                if (chkCoeEVs.Checked)
                    intNCoeff = matCoe.RowCount;
                else
                {
                    intNCoeff = matCoe.RowCount - intNSelectedEVs;
                }


                //Store Data Table by R result
                for (int j = 0; j < intNCoeff; j++)
                {
                    DataRow pDataRow = tblRegResult.NewRow();
                    if (j == 0 && intInterceptModel != 1)
                    {
                        pDataRow["Name"] = "(Intercept)";
                    }
                    else
                    {
                        if (chkCoeEVs.Checked)
                            pDataRow["Name"] = vecNames[j];
                        else
                        {
                            if (intInterceptModel == 1)
                                pDataRow["Name"] = independentNames[j];
                            else
                                pDataRow["Name"] = independentNames[j - 1];
                        }
                    }
                    pDataRow["Estimate"] = Math.Round(matCoe[j, 0], intDeciPlaces);
                    pDataRow["Std. Error"] = Math.Round(matCoe[j, 1], intDeciPlaces);
                    pDataRow["z value"] = Math.Round(matCoe[j, 2], intDeciPlaces);
                    pDataRow["Pr(>|z|)"] = Math.Round(matCoe[j, 3], intDeciPlaces);
                    tblRegResult.Rows.Add(pDataRow);
                }

                //Assign Datagridview to Data Table
                pfrmRegResult.dgvResults.DataSource = tblRegResult;

                int nFeature = pFLayer.FeatureClass.FeatureCount(null);

                //Get EV and SA for selected filters
                NumericMatrix nmModel = m_pEngine.Evaluate("as.matrix(sample.esf$model)").AsNumericMatrix();
                double[] arrFilter = new double[nFeature];
                
                //Get individual the selected EVs (v 1.0.6)
                Double[,] arrSelectedEVs = new double[nFeature, intNSelectedEVs];
                String[] arrSelectedEvsNM = null;
                if (intInterceptModel == 1)
                    arrSelectedEvsNM = vecNames.Skip(nIDepen).ToArray();
                else
                    arrSelectedEvsNM = vecNames.Skip(nIDepen+1).ToArray();



                IFeatureCursor pFCursor = pFLayer.FeatureClass.Search(null, false);
                IFeature pFeature = pFCursor.NextFeature();
                int featureIdx = 0;

                while (pFeature != null)
                {
                    //Calculate and update spatial filter (Coefficient estimate * selected EVs)
                    double dblIntMedValue = 0;
                    double dblFilterValue = 0;
                    for (int k = 0; k < intNSelectedEVs; k++)
                    {
                        if (intInterceptModel == 1)
                        {
                            arrSelectedEVs[featureIdx, k] = nmModel[featureIdx, nIDepen + k]; //Get individual the selected EVs (v 1.0.6)
                            dblIntMedValue = matCoe[nIDepen + k, 0] * nmModel[featureIdx, nIDepen + k];
                        }
                        else
                        {
                            arrSelectedEVs[featureIdx, k] = nmModel[featureIdx, nIDepen + k + 1];//Get individual the selected EVs (v 1.0.6)
                            dblIntMedValue = matCoe[nIDepen + k + 1, 0] * nmModel[featureIdx, nIDepen + k + 1];
                        }



                        dblFilterValue += dblIntMedValue;
                    }

                    arrFilter[featureIdx] = dblFilterValue;
                    pFeature = pFCursor.NextFeature();
                    featureIdx++;
                }

                //Assign the EV and their names into the result form (v1.0.6)
                pfrmRegResult.arrSelectedEVs = arrSelectedEVs;
                pfrmRegResult.arrSelectedEvsNM = arrSelectedEvsNM;
                pfrmRegResult.m_pFClass = m_pFClass;

                //Calculate MC for the constructed ESF
                NumericVector vecFilter = null;
                double dblSfilterMC = 0;
                double dblSfilterpVal = 0;

                if (intNSelectedEVs > 0)
                {
                    vecFilter = m_pEngine.CreateNumericVector(arrFilter);
                    m_pEngine.SetSymbol("samplefilter", vecFilter);

                    if (cboAlternative.Text == "Greater")
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'greater', zero.policy=TRUE)");
                    else if (cboAlternative.Text == "Less")
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'less', zero.policy=TRUE)");
                    else
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'greater', zero.policy=TRUE)");

                    //m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, zero.policy=TRUE)");
                    dblSfilterMC = m_pEngine.Evaluate("sfilter.mc$estimate").AsNumeric().First();
                    dblSfilterpVal = m_pEngine.Evaluate("sfilter.mc$p.value").AsNumeric().First();
                }

                //Assign values at Textbox
                string strDecimalPlaces = "N" + intDeciPlaces.ToString();
                string[] strResults = new string[8];
                strResults[0] = "Number of rows: " + nFeature.ToString() + ", Number of candidate EVs: " + dblNCandidateEvs.ToString() + ", Selected EVs: " + intNSelectedEVs.ToString();

                if (dblResiLMpVal < 0.001)
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value < 0.001";
                else if (dblResiLMpVal > 0.999)
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value > 0.999";
                else
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value: " + dblResiLMpVal.ToString("N3");

                strResults[2] = "AIC of Final Model: " + nvecNonAIC.Last().ToString(strDecimalPlaces);
                //strResults[2] = "AIC of non-ESF: " + m_pEngine.Evaluate("AIC(esf.org)").AsNumeric().First().ToString(strDecimalPlaces) + ", AIC of Final Model: " + m_pEngine.Evaluate("AIC(sample.esf)").AsNumeric().First().ToString(strDecimalPlaces);

                strResults[3] = "Null deviance: " + dblNullDevi.ToString(strDecimalPlaces) + " on " + dblNullDF.ToString("N0") + " degrees of freedom";
                strResults[4] = "Residual deviance: " + dblResiDevi.ToString(strDecimalPlaces) + " on " + dblResiDF.ToString("N0") + " degrees of freedom";

                //if (intInterceptModel != 1)
                //    strResults[5] = "Nagelkerke pseudo R squared: " + dblPseudoRsquared.ToString(strDecimalPlaces);
                //else
                //    strResults[5] = "";
                strResults[5] = "Pseudo R squared: " + dblPseudoRsquared.ToString(strDecimalPlaces);

                if (dblResiMCpVal < 0.001)
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value < 0.001";
                else if (dblResiMCpVal > 0.999)
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value > 0.999";
                else
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value: " + dblResiMCpVal.ToString("N3");

                if (intNSelectedEVs > 0)
                {
                    if (dblSfilterpVal < 0.001)
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value < 0.001";
                    else if (dblSfilterpVal > 0.999)
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value > 0.999";
                    else
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value: " + dblSfilterpVal.ToString(strDecimalPlaces);
                }
                else
                    strResults[7] = "";

                pfrmRegResult.txtOutput.Lines = strResults;

                //Save Outputs in SHP
                if (chkSave.Checked)
                {
                    pfrmProgress.lblStatus.Text = "Saving residuals and spatial filter:";
                    //The field names are related with string[] DeterminedName in clsSnippet 
                    string strResiFldName = lstSave.Items[0].SubItems[1].Text;
                    string strSFilterName = lstSave.Items[1].SubItems[1].Text;


                    //Get EVs and residuals
                    NumericVector nvResiduals = m_pEngine.Evaluate("as.numeric(residuals(sample.esf, type='pearson'))").AsNumeric(); //Pearson Residual

                    // Create field, if there isn't
                    // Create field, if there isn't
                    if (m_pFClass.FindField(strResiFldName) == -1)
                    {
                        //Add fields
                        IField newField = new FieldClass();
                        IFieldEdit fieldEdit = (IFieldEdit)newField;
                        fieldEdit.Name_2 = strResiFldName;
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        m_pFClass.AddField(newField);
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Do you want to overwrite " + strResiFldName + " field?", "Overwrite", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (m_pFClass.FindField(strSFilterName) == -1)
                    {
                        IField newField = new FieldClass();
                        IFieldEdit fieldEdit = (IFieldEdit)newField;
                        fieldEdit.Name_2 = strSFilterName;
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        m_pFClass.AddField(newField);
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Do you want to overwrite " + strSFilterName + " field?", "Overwrite", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }

                    //Update Field
                    pFCursor = pFLayer.FeatureClass.Update(null, false);
                    pFeature = pFCursor.NextFeature();

                    featureIdx = 0;
                    int intResiFldIdx = m_pFClass.FindField(strResiFldName);
                    int intSFilterIdx = m_pFClass.FindField(strSFilterName);

                    while (pFeature != null)
                    {
                        //Update Residuals
                        pFeature.set_Value(intResiFldIdx, (object)nvResiduals[featureIdx]);
                        if (intNSelectedEVs > 0)
                            pFeature.set_Value(intSFilterIdx, (object)vecFilter[featureIdx]);

                        pFCursor.UpdateFeature(pFeature);

                        pFeature = pFCursor.NextFeature();
                        featureIdx++;
                    }

                }


                pfrmRegResult.Show();
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog(); pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                pfrmProgress.Close();
                return;
            }
        }

        private void BinomESF(frmProgress pfrmProgress, IFeatureLayer pFLayer, string strLM, int nIDepen, String[] independentNames, double dblNCandidateEvs, int intDeciPlaces, int intInterceptModel)
        {
            try
            {

                pfrmProgress.lblStatus.Text = "Selecting EVs";
                m_pEngine.Evaluate("esf.full <- glm(" + strLM + "+., data=EV, family='binomial')");
                m_pEngine.Evaluate("esf.org <- glm(" + strLM + ", data=EV, family='binomial')");
                //m_pEngine.Evaluate("sample.esf <- stepAIC(esf.org, scope=list(upper= esf.full), direction='forward')");
                //m_pEngine.Evaluate("sample.esf <- stepwise(esf.full, esf.org, verbose=FALSE)");
                m_pEngine.Evaluate("sample.esf <- stepAIC(esf.org, scope=list(lower = esf.org, upper= esf.full), verbose = FALSE)");

                pfrmProgress.lblStatus.Text = "Printing Output:";
                m_pEngine.Evaluate("sum.esf <- summary(sample.esf)");
                
                NumericMatrix matCoe = m_pEngine.Evaluate("as.matrix(sum.esf$coefficient)").AsNumericMatrix();
                CharacterVector vecNames = m_pEngine.Evaluate("attributes(sum.esf$coefficients)$dimnames[[1]]").AsCharacter();
                double dblNullDevi = m_pEngine.Evaluate("sum.esf$null.deviance").AsNumeric().First();
                double dblNullDF = m_pEngine.Evaluate("sum.esf$df.null").AsNumeric().First();
                double dblResiDevi = m_pEngine.Evaluate("sum.esf$deviance").AsNumeric().First();
                double dblResiDF = m_pEngine.Evaluate("sum.esf$df.residual").AsNumeric().First();

                //Nagelkerke r squared
                //double dblPseudoRsquared = m_pEngine.Evaluate("(1 - exp((sample.esf$dev - sample.esf$null)/sample.n))/(1 - exp(-sample.esf$null/sample.n))").AsNumeric().First();
                double dblPseudoRsquared = m_pEngine.Evaluate("summary(lm(sample.esf$y~sample.esf$fitted.values))$r.squared").AsNumeric().First();

                //double dblResiMC = m_pEngine.Evaluate("moran.test(sample.esf$residuals, sample.listw)$estimate").AsNumeric().First();
                //double dblResiMCpVal = m_pEngine.Evaluate("moran.test(sample.esf$residuals, sample.listw)$p.value").AsNumeric().First();
                //double dblResiLMMC = m_pEngine.Evaluate("moran.test(esf.org$residuals, sample.listw)$estimate").AsNumeric().First();
                //double dblResiLMpVal = m_pEngine.Evaluate("moran.test(esf.org$residuals, sample.listw)$p.value").AsNumeric().First();

                //MC Using Pearson residual (Lin and Zhang 2007, GA) 
                if (cboAlternative.Text == "Greater")
                    m_pEngine.Evaluate("sampleresi.mc <-moran.mc(residuals(sample.esf, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Less")
                    m_pEngine.Evaluate("sampleresi.mc <-moran.mc(residuals(sample.esf, type='pearson'), listw =sample.listb, nsim=999, alternative = 'less', zero.policy=TRUE)");
                else
                    m_pEngine.Evaluate("sampleresi.mc <-moran.mc(residuals(sample.esf, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");

                double dblResiMC = m_pEngine.Evaluate("sampleresi.mc$statistic").AsNumeric().First();
                double dblResiMCpVal = m_pEngine.Evaluate("sampleresi.mc$p.value").AsNumeric().First();

                if (cboAlternative.Text == "Greater")
                    m_pEngine.Evaluate("orgresi.mc <-moran.mc(residuals(esf.org, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Less")
                    m_pEngine.Evaluate("orgresi.mc <-moran.mc(residuals(esf.org, type='pearson'), listw =sample.listb, nsim=999, alternative = 'less', zero.policy=TRUE)");
                else
                    m_pEngine.Evaluate("orgresi.mc <-moran.mc(residuals(esf.org, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");

                double dblResiLMMC = m_pEngine.Evaluate("orgresi.mc$statistic").AsNumeric().First();
                double dblResiLMpVal = m_pEngine.Evaluate("orgresi.mc$p.value").AsNumeric().First();

                NumericVector nvecNonAIC = m_pEngine.Evaluate("sample.esf$aic").AsNumeric();

                //Open Ouput form
                frmESFResult pfrmRegResult = new frmESFResult();
                pfrmRegResult.Text = "ESF Binomial Regression Summary";

                //Create DataTable to store Result
                System.Data.DataTable tblRegResult = new DataTable("ESFResult");

                //Assign DataTable
                DataColumn dColName = new DataColumn();
                dColName.DataType = System.Type.GetType("System.String");
                dColName.ColumnName = "Name";
                tblRegResult.Columns.Add(dColName);

                DataColumn dColValue = new DataColumn();
                dColValue.DataType = System.Type.GetType("System.Double");
                dColValue.ColumnName = "Estimate";
                tblRegResult.Columns.Add(dColValue);

                DataColumn dColSE = new DataColumn();
                dColSE.DataType = System.Type.GetType("System.Double");
                dColSE.ColumnName = "Std. Error";
                tblRegResult.Columns.Add(dColSE);

                DataColumn dColTValue = new DataColumn();
                dColTValue.DataType = System.Type.GetType("System.Double");
                dColTValue.ColumnName = "z value";
                tblRegResult.Columns.Add(dColTValue);

                DataColumn dColPvT = new DataColumn();
                dColPvT.DataType = System.Type.GetType("System.Double");
                dColPvT.ColumnName = "Pr(>|z|)";
                tblRegResult.Columns.Add(dColPvT);

                int intNCoeff = 0;
                int intNSelectedEVs = matCoe.RowCount - nIDepen - 1;
                if (intInterceptModel == 1)
                    intNSelectedEVs = matCoe.RowCount - nIDepen;

                if (chkCoeEVs.Checked)
                    intNCoeff = matCoe.RowCount;
                else
                {
                    intNCoeff = matCoe.RowCount - intNSelectedEVs;
                }


                //Store Data Table by R result
                for (int j = 0; j < intNCoeff; j++)
                {
                    DataRow pDataRow = tblRegResult.NewRow();
                    if (j == 0 && intInterceptModel != 1)
                    {
                        pDataRow["Name"] = "(Intercept)";
                    }
                    else
                    {
                        if (chkCoeEVs.Checked)
                            pDataRow["Name"] = vecNames[j];
                        else
                        {
                            if (intInterceptModel == 1)
                                pDataRow["Name"] = independentNames[j];
                            else
                                pDataRow["Name"] = independentNames[j - 1];
                        }

                    }
                    pDataRow["Estimate"] = Math.Round(matCoe[j, 0], intDeciPlaces);
                    pDataRow["Std. Error"] = Math.Round(matCoe[j, 1], intDeciPlaces);
                    pDataRow["z value"] = Math.Round(matCoe[j, 2], intDeciPlaces);
                    pDataRow["Pr(>|z|)"] = Math.Round(matCoe[j, 3], intDeciPlaces);
                    tblRegResult.Rows.Add(pDataRow);
                }

                //Assign Datagridview to Data Table
                pfrmRegResult.dgvResults.DataSource = tblRegResult;

                int nFeature = pFLayer.FeatureClass.FeatureCount(null);

                //Get EV and SA for selected filters
                NumericMatrix nmModel = m_pEngine.Evaluate("as.matrix(sample.esf$model)").AsNumericMatrix();
                double[] arrFilter = new double[nFeature];

                //Get individual the selected EVs (v 1.0.6)
                Double[,] arrSelectedEVs = new double[nFeature, intNSelectedEVs];
                String[] arrSelectedEvsNM = null;
                if (intInterceptModel == 1)
                    arrSelectedEvsNM = vecNames.Skip(nIDepen).ToArray();
                else
                    arrSelectedEvsNM = vecNames.Skip(nIDepen + 1).ToArray();

                IFeatureCursor pFCursor = pFLayer.FeatureClass.Search(null, false);
                IFeature pFeature = pFCursor.NextFeature();

                int featureIdx = 0;

                while (pFeature != null)
                {
                    //Calculate and update spatial filter (Coefficient estimate * selected EVs)
                    double dblIntMedValue = 0;
                    double dblFilterValue = 0;
                    for (int k = 0; k < intNSelectedEVs; k++)
                    {
                        if (intInterceptModel == 1)
                        {
                            arrSelectedEVs[featureIdx, k] = nmModel[featureIdx, nIDepen + k]; //Get individual the selected EVs (v 1.0.6)
                            dblIntMedValue = matCoe[nIDepen + k, 0] * nmModel[featureIdx, nIDepen + k];
                        }
                        else
                        {
                            arrSelectedEVs[featureIdx, k] = nmModel[featureIdx, nIDepen + k + 1];//Get individual the selected EVs (v 1.0.6)
                            dblIntMedValue = matCoe[nIDepen + k + 1, 0] * nmModel[featureIdx, nIDepen + k + 1];
                        }


                        dblFilterValue += dblIntMedValue;
                    }
                    arrFilter[featureIdx] = dblFilterValue;
                    pFeature = pFCursor.NextFeature();
                    featureIdx++;
                }

                //Assign the EV and their names into the result form (v1.0.6)
                pfrmRegResult.arrSelectedEVs = arrSelectedEVs;
                pfrmRegResult.arrSelectedEvsNM = arrSelectedEvsNM;
                pfrmRegResult.m_pFClass = m_pFClass;


                //Calculate MC for the constructed ESF
                NumericVector vecFilter = null;
                double dblSfilterMC = 0;
                double dblSfilterpVal = 0;

                if (intNSelectedEVs > 0)
                {
                    vecFilter = m_pEngine.CreateNumericVector(arrFilter);
                    m_pEngine.SetSymbol("samplefilter", vecFilter);

                    if (cboAlternative.Text == "Greater")
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'greater', zero.policy=TRUE)");
                    else if (cboAlternative.Text == "Less")
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'less', zero.policy=TRUE)");
                    else
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'greater', zero.policy=TRUE)");

                    //m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, zero.policy=TRUE)");
                    dblSfilterMC = m_pEngine.Evaluate("sfilter.mc$estimate").AsNumeric().First();
                    dblSfilterpVal = m_pEngine.Evaluate("sfilter.mc$p.value").AsNumeric().First();
                }

                //Assign values at Textbox
                string strDecimalPlaces = "N" + intDeciPlaces.ToString();
                string[] strResults = new string[8];
                strResults[0] = "Number of rows: " + nFeature.ToString() + ", Number of candidate EVs: " + dblNCandidateEvs.ToString() + ", Selected EVs: " + intNSelectedEVs.ToString();
                if (dblResiLMpVal < 0.001)
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value < 0.001";
                else if (dblResiLMpVal > 0.999)
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value > 0.999";
                else
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value: " + dblResiLMpVal.ToString("N3");

                strResults[2] = "AIC of Final Model: " + nvecNonAIC.Last().ToString(strDecimalPlaces);
                //strResults[2] = "AIC of non-ESF: " + m_pEngine.Evaluate("AIC(esf.org)").AsNumeric().First().ToString(strDecimalPlaces) + ", AIC of Final Model: " + m_pEngine.Evaluate("AIC(sample.esf)").AsNumeric().First().ToString(strDecimalPlaces);

                strResults[3] = "Null deviance: " + dblNullDevi.ToString(strDecimalPlaces) + " on " + dblNullDF.ToString("N0") + " degrees of freedom";
                strResults[4] = "Residual deviance: " + dblResiDevi.ToString(strDecimalPlaces) + " on " + dblResiDF.ToString("N0") + " degrees of freedom";

                //if (intInterceptModel != 1)
                //    strResults[5] = "Nagelkerke pseudo R squared: " + dblPseudoRsquared.ToString(strDecimalPlaces);
                //else
                //    strResults[5] = "";
                strResults[5] = "Pseudo R squared: " + dblPseudoRsquared.ToString(strDecimalPlaces);

                if (dblResiMCpVal < 0.001)
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value < 0.001";
                else if (dblResiMCpVal > 0.999)
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value > 0.999";
                else
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value: " + dblResiMCpVal.ToString("N3");

                if (intNSelectedEVs > 0)
                {
                    if (dblSfilterpVal < 0.001)
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value < 0.001";
                    else if (dblSfilterpVal > 0.999)
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value > 0.999";
                    else
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value: " + dblSfilterpVal.ToString(strDecimalPlaces);
                }
                else
                    strResults[7] = "";

                pfrmRegResult.txtOutput.Lines = strResults;

                //Save Outputs in SHP
                if (chkSave.Checked)
                {
                    pfrmProgress.lblStatus.Text = "Saving residuals and spatial filter:";
                    //The field names are related with string[] DeterminedName in clsSnippet 
                    string strResiFldName = lstSave.Items[0].SubItems[1].Text;
                    string strSFilterName = lstSave.Items[1].SubItems[1].Text;


                    //Get EVs and residuals
                    NumericVector nvResiduals = m_pEngine.Evaluate("as.numeric(residuals(sample.esf, type='pearson'))").AsNumeric(); //Pearson Residual

                    // Create field, if there isn't
                    if (m_pFClass.FindField(strResiFldName) == -1)
                    {
                        //Add fields
                        IField newField = new FieldClass();
                        IFieldEdit fieldEdit = (IFieldEdit)newField;
                        fieldEdit.Name_2 = strResiFldName;
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        m_pFClass.AddField(newField);
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Do you want to overwrite " + strResiFldName + " field?", "Overwrite", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (m_pFClass.FindField(strSFilterName) == -1)
                    {
                        IField newField = new FieldClass();
                        IFieldEdit fieldEdit = (IFieldEdit)newField;
                        fieldEdit.Name_2 = strSFilterName;
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        m_pFClass.AddField(newField);
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Do you want to overwrite " + strSFilterName + " field?", "Overwrite", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }

                    //Update Field
                    pFCursor = pFLayer.FeatureClass.Update(null, false);
                    pFeature = pFCursor.NextFeature();

                    featureIdx = 0;
                    int intResiFldIdx = m_pFClass.FindField(strResiFldName);
                    int intSFilterIdx = m_pFClass.FindField(strSFilterName);

                    while (pFeature != null)
                    {
                        //Update Residuals and spatial filter
                        pFeature.set_Value(intResiFldIdx, (object)nvResiduals[featureIdx]);
                        if (intNSelectedEVs > 0)
                            pFeature.set_Value(intSFilterIdx, (object)vecFilter[featureIdx]);

                        pFCursor.UpdateFeature(pFeature);

                        pFeature = pFCursor.NextFeature();
                        featureIdx++;
                    }

                }

                pfrmRegResult.Show();
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog(); pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                pfrmProgress.Close();
                return;
            }
        }

        private void PoissonESF(frmProgress pfrmProgress, IFeatureLayer pFLayer, string strLM, int nIDepen, String[] independentNames, string strNoramlName, double dblNCandidateEvs, int intDeciPlaces, int intInterceptModel)
        {
            try
            {

                pfrmProgress.lblStatus.Text = "Selecting EVs";
                if (strNoramlName == "")
                {
                    m_pEngine.Evaluate("esf.full <- glm(" + strLM + "+., data=EV, family='poisson')");
                    m_pEngine.Evaluate("esf.org <- glm(" + strLM + ", data=EV, family='poisson')");
                }
                else
                {
                    try
                    {
                        m_pEngine.Evaluate("esf.full <- glm(" + strLM + "+., offset=" + strNoramlName + ", data=EV, family='poisson')");
                        m_pEngine.Evaluate("esf.org <- glm(" + strLM + ", offset=" + strNoramlName + ", data=EV, family='poisson')");
                    }
                    catch
                    {
                        MessageBox.Show("An offset value requires a logarithm form. Please check the model again.");
                        pfrmProgress.Close();
                        return;
                    }

                }
                //m_pEngine.Evaluate("sample.esf <- stepAIC(esf.org, scope=list(upper= esf.full), direction='forward')");
                //m_pEngine.Evaluate("sample.esf <- stepwise(esf.full, esf.org, verbose=FALSE)");
                m_pEngine.Evaluate("sample.esf <- stepAIC(esf.org, scope=list(lower = esf.org, upper= esf.full), verbose = FALSE)");

                pfrmProgress.lblStatus.Text = "Printing Output:";
                m_pEngine.Evaluate("sum.esf <- summary(sample.esf)");
                //m_pEngine.Evaluate("sample.lm <- lm(" + strLM + ")");

                NumericMatrix matCoe = m_pEngine.Evaluate("as.matrix(sum.esf$coefficient)").AsNumericMatrix();
                CharacterVector vecNames = m_pEngine.Evaluate("attributes(sum.esf$coefficients)$dimnames[[1]]").AsCharacter();

                double dblNullDevi = m_pEngine.Evaluate("sum.esf$null.deviance").AsNumeric().First();
                double dblNullDF = m_pEngine.Evaluate("sum.esf$df.null").AsNumeric().First();
                double dblResiDevi = m_pEngine.Evaluate("sum.esf$deviance").AsNumeric().First();
                double dblResiDF = m_pEngine.Evaluate("sum.esf$df.residual").AsNumeric().First();

                //MC Using Pearson residual (Lin and Zhang 2007, GA) 
                if (cboAlternative.Text == "Greater")
                    m_pEngine.Evaluate("sampleresi.mc <-moran.mc(residuals(sample.esf, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Less")
                    m_pEngine.Evaluate("sampleresi.mc <-moran.mc(residuals(sample.esf, type='pearson'), listw =sample.listb, nsim=999, alternative = 'less', zero.policy=TRUE)");
                else
                    m_pEngine.Evaluate("sampleresi.mc <-moran.mc(residuals(sample.esf, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");

                double dblResiMC = m_pEngine.Evaluate("sampleresi.mc$statistic").AsNumeric().First();
                double dblResiMCpVal = m_pEngine.Evaluate("sampleresi.mc$p.value").AsNumeric().First();

                if (cboAlternative.Text == "Greater")
                    m_pEngine.Evaluate("orgresi.mc <-moran.mc(residuals(esf.org, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Less")
                    m_pEngine.Evaluate("orgresi.mc <-moran.mc(residuals(esf.org, type='pearson'), listw =sample.listb, nsim=999, alternative = 'less', zero.policy=TRUE)");
                else
                    m_pEngine.Evaluate("orgresi.mc <-moran.mc(residuals(esf.org, type='pearson'), listw =sample.listb, nsim=999, alternative = 'greater', zero.policy=TRUE)");

                double dblResiLMMC = m_pEngine.Evaluate("orgresi.mc$statistic").AsNumeric().First();
                double dblResiLMpVal = m_pEngine.Evaluate("orgresi.mc$p.value").AsNumeric().First();
                //Nagelkerke r squared
                //double dblPseudoRsquared = m_pEngine.Evaluate("(1 - exp((sample.esf$deviance - sample.esf$null.deviance)/sample.n))/(1 - exp(-sample.esf$null.deviance/sample.n))").AsNumeric().First();
                double dblPseudoRsquared = m_pEngine.Evaluate("summary(lm(sample.esf$y~sample.esf$fitted.values))$r.squared").AsNumeric().First();

                NumericVector nvecNonAIC = m_pEngine.Evaluate("sample.esf$aic").AsNumeric();

                //Open Ouput form
                frmESFResult pfrmRegResult = new frmESFResult();
                if (strNoramlName == "")
                    pfrmRegResult.Text = "ESF Poisson Regression Summary";
                else
                    pfrmRegResult.Text = "ESF Poisson Regression with Offset (" + strNoramlName + ") Summary";

                //Create DataTable to store Result
                System.Data.DataTable tblRegResult = new DataTable("ESFResult");

                //Assign DataTable
                DataColumn dColName = new DataColumn();
                dColName.DataType = System.Type.GetType("System.String");
                dColName.ColumnName = "Name";
                tblRegResult.Columns.Add(dColName);

                DataColumn dColValue = new DataColumn();
                dColValue.DataType = System.Type.GetType("System.Double");
                dColValue.ColumnName = "Estimate";
                tblRegResult.Columns.Add(dColValue);

                DataColumn dColSE = new DataColumn();
                dColSE.DataType = System.Type.GetType("System.Double");
                dColSE.ColumnName = "Std. Error";
                tblRegResult.Columns.Add(dColSE);

                DataColumn dColTValue = new DataColumn();
                dColTValue.DataType = System.Type.GetType("System.Double");
                dColTValue.ColumnName = "z value";
                tblRegResult.Columns.Add(dColTValue);

                DataColumn dColPvT = new DataColumn();
                dColPvT.DataType = System.Type.GetType("System.Double");
                dColPvT.ColumnName = "Pr(>|z|)";
                tblRegResult.Columns.Add(dColPvT);

                int intNCoeff = 0;
                int intNSelectedEVs = matCoe.RowCount - nIDepen - 1;
                if (intInterceptModel == 1)
                    intNSelectedEVs = matCoe.RowCount - nIDepen;

                if (chkCoeEVs.Checked)
                    intNCoeff = matCoe.RowCount;
                else
                {
                    intNCoeff = matCoe.RowCount - intNSelectedEVs;
                }


                //Store Data Table by R result
                for (int j = 0; j < intNCoeff; j++)
                {
                    DataRow pDataRow = tblRegResult.NewRow();
                    if (j == 0 && intInterceptModel != 1)
                    {
                        pDataRow["Name"] = "(Intercept)";
                    }
                    else
                    {
                        if (chkCoeEVs.Checked)
                            pDataRow["Name"] = vecNames[j];
                        else
                        {
                            if (intInterceptModel == 1)
                                pDataRow["Name"] = independentNames[j];
                            else
                                pDataRow["Name"] = independentNames[j - 1];
                        }
                    }
                    pDataRow["Estimate"] = Math.Round(matCoe[j, 0], intDeciPlaces);
                    pDataRow["Std. Error"] = Math.Round(matCoe[j, 1], intDeciPlaces);
                    pDataRow["z value"] = Math.Round(matCoe[j, 2], intDeciPlaces);
                    pDataRow["Pr(>|z|)"] = Math.Round(matCoe[j, 3], intDeciPlaces);
                    tblRegResult.Rows.Add(pDataRow);
                }

                //Assign Datagridview to Data Table
                pfrmRegResult.dgvResults.DataSource = tblRegResult;

                int nFeature = pFLayer.FeatureClass.FeatureCount(null);

                //Get EV and SA for selected filters
                NumericMatrix nmModel = m_pEngine.Evaluate("as.matrix(sample.esf$model)").AsNumericMatrix();
                double[] arrFilter = new double[nFeature];

                //Get individual the selected EVs (v 1.0.6)
                Double[,] arrSelectedEVs = new double[nFeature, intNSelectedEVs];
                String[] arrSelectedEvsNM = null;
                if (intInterceptModel == 1)
                    arrSelectedEvsNM = vecNames.Skip(nIDepen).ToArray();
                else
                    arrSelectedEvsNM = vecNames.Skip(nIDepen + 1).ToArray();

                IFeatureCursor pFCursor = pFLayer.FeatureClass.Search(null, false);
                IFeature pFeature = pFCursor.NextFeature();

                int featureIdx = 0;

                while (pFeature != null)
                {
                    //Calculate and update spatial filter (Coefficient estimate * selected EVs)
                    double dblIntMedValue = 0;
                    double dblFilterValue = 0;
                    for (int k = 0; k < intNSelectedEVs; k++)
                    {
                        if (intInterceptModel == 1)
                        {
                            arrSelectedEVs[featureIdx, k] = nmModel[featureIdx, nIDepen + k]; //Get individual the selected EVs (v 1.0.6)
                            dblIntMedValue = matCoe[nIDepen + k, 0] * nmModel[featureIdx, nIDepen + k];
                        }
                        else
                        {
                            arrSelectedEVs[featureIdx, k] = nmModel[featureIdx, nIDepen + k + 1];//Get individual the selected EVs (v 1.0.6)
                            dblIntMedValue = matCoe[nIDepen + k + 1, 0] * nmModel[featureIdx, nIDepen + k + 1];
                        }

                        dblFilterValue += dblIntMedValue;
                    }

                    arrFilter[featureIdx] = dblFilterValue;
                    pFeature = pFCursor.NextFeature();
                    featureIdx++;
                }

                //Assign the EV and their names into the result form (v1.0.6)
                pfrmRegResult.arrSelectedEVs = arrSelectedEVs;
                pfrmRegResult.arrSelectedEvsNM = arrSelectedEvsNM;
                pfrmRegResult.m_pFClass = m_pFClass;


                //Calculate MC for the constructed ESF
                NumericVector vecFilter = null;
                double dblSfilterMC = 0;
                double dblSfilterpVal = 0;

                if (intNSelectedEVs > 0)
                {
                    vecFilter = m_pEngine.CreateNumericVector(arrFilter);
                    m_pEngine.SetSymbol("samplefilter", vecFilter);

                    if (cboAlternative.Text == "Greater")
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'greater', zero.policy=TRUE)");
                    else if (cboAlternative.Text == "Less")
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'less', zero.policy=TRUE)");
                    else
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'greater', zero.policy=TRUE)");

                    //m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, zero.policy=TRUE)");
                    dblSfilterMC = m_pEngine.Evaluate("sfilter.mc$estimate").AsNumeric().First();
                    dblSfilterpVal = m_pEngine.Evaluate("sfilter.mc$p.value").AsNumeric().First();
                }

                //Assign values at Textbox
                string strDecimalPlaces = "N" + intDeciPlaces.ToString();
                string[] strResults = new string[8];
                strResults[0] = "Number of rows: " + nFeature.ToString() + ", Number of candidate EVs: " + dblNCandidateEvs.ToString() + ", Selected EVs: " + intNSelectedEVs.ToString();

                if (dblResiLMpVal < 0.001)
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value < 0.001";
                else if (dblResiLMpVal > 0.999)
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value > 0.999";
                else
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value: " + dblResiLMpVal.ToString("N3");

                strResults[2] = "AIC of Final Model: " + nvecNonAIC.Last().ToString(strDecimalPlaces);
                //strResults[2] = "AIC of non-ESF: " + m_pEngine.Evaluate("AIC(esf.org)").AsNumeric().First().ToString(strDecimalPlaces) + ", AIC of Final Model: " + m_pEngine.Evaluate("AIC(sample.esf)").AsNumeric().First().ToString(strDecimalPlaces);

                strResults[3] = "Null deviance: " + dblNullDevi.ToString(strDecimalPlaces) + " on " + dblNullDF.ToString("N0") + " degrees of freedom";
                strResults[4] = "Residual deviance: " + dblResiDevi.ToString(strDecimalPlaces) + " on " + dblResiDF.ToString("N0") + " degrees of freedom";

                //if (intInterceptModel != 1)
                //    strResults[5] = "Nagelkerke pseudo R squared: " + dblPseudoRsquared.ToString(strDecimalPlaces);
                //else
                //    strResults[5] = "";
                strResults[5] = "Pseudo R squared: " + dblPseudoRsquared.ToString(strDecimalPlaces);

                if (dblResiMCpVal < 0.001)
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value < 0.001";
                else if (dblResiMCpVal > 0.999)
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value > 0.999";
                else
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value: " + dblResiMCpVal.ToString("N3");

                if (intNSelectedEVs > 0)
                {
                    if (dblSfilterpVal < 0.001)
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value < 0.001";
                    else if (dblSfilterpVal > 0.999)
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value > 0.999";
                    else
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value: " + dblSfilterpVal.ToString(strDecimalPlaces);
                }
                else
                    strResults[7] = "";

                pfrmRegResult.txtOutput.Lines = strResults;

                //Save Outputs in SHP
                if (chkSave.Checked)
                {
                    pfrmProgress.lblStatus.Text = "Saving residuals and spatial filter:";
                    //The field names are related with string[] DeterminedName in clsSnippet 
                    string strResiFldName = lstSave.Items[0].SubItems[1].Text;
                    string strSFilterName = lstSave.Items[1].SubItems[1].Text;


                    //Get EVs and residuals
                    NumericVector nvResiduals = m_pEngine.Evaluate("as.numeric(residuals(sample.esf, type='pearson'))").AsNumeric(); //Pearson Residual

                    // Create field, if there isn't
                    // Create field, if there isn't
                    if (m_pFClass.FindField(strResiFldName) == -1)
                    {
                        //Add fields
                        IField newField = new FieldClass();
                        IFieldEdit fieldEdit = (IFieldEdit)newField;
                        fieldEdit.Name_2 = strResiFldName;
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        m_pFClass.AddField(newField);
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Do you want to overwrite " + strResiFldName + " field?", "Overwrite", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (m_pFClass.FindField(strSFilterName) == -1)
                    {
                        IField newField = new FieldClass();
                        IFieldEdit fieldEdit = (IFieldEdit)newField;
                        fieldEdit.Name_2 = strSFilterName;
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        m_pFClass.AddField(newField);
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Do you want to overwrite " + strSFilterName + " field?", "Overwrite", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }

                    //Update Field
                    pFCursor = pFLayer.FeatureClass.Update(null, false);
                    pFeature = pFCursor.NextFeature();

                    featureIdx = 0;
                    int intResiFldIdx = m_pFClass.FindField(strResiFldName);
                    int intSFilterIdx = m_pFClass.FindField(strSFilterName);

                    while (pFeature != null)
                    {
                        //Update Residuals
                        pFeature.set_Value(intResiFldIdx, (object)nvResiduals[featureIdx]);
                        if (intNSelectedEVs > 0)
                            pFeature.set_Value(intSFilterIdx, (object)vecFilter[featureIdx]);

                        pFCursor.UpdateFeature(pFeature);

                        pFeature = pFCursor.NextFeature();
                        featureIdx++;
                    }

                }


                pfrmRegResult.Show();
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog(); pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                pfrmProgress.Close();
                return;
            }
        }

        private void LinearESF(frmProgress pfrmProgress, IFeatureLayer pFLayer, string strLM, int nIDepen, String[] independentNames, double dblNCandidateEvs, int intDeciPlaces, int intInterceptModel)
        {
            try
            {

                pfrmProgress.lblStatus.Text = "Selecting EVs";
                m_pEngine.Evaluate("esf.full <- lm(" + strLM + "+., data=EV)");
                //m_pEngine.Evaluate("sample.esf <- stepAIC(lm(" + strLM + ", data=EV), scope=list(upper= esf.full), direction='forward')");
                m_pEngine.Evaluate("sample.esf <- stepAIC(lm(" + strLM + ", data=EV), scope=list(lower = lm(" + strLM + ", data=EV), upper= esf.full), verbose = FALSE)");
                //m_pEngine.Evaluate("sample.esf <- stepwise(esf.full, lm(" + strLM + ", data=EV), verbose=FALSE)");

                pfrmProgress.lblStatus.Text = "Printing Output:";
                m_pEngine.Evaluate("sum.esf <- summary(sample.esf)");
                m_pEngine.Evaluate("sample.lm <- lm(" + strLM + ")");

                NumericMatrix matCoe = m_pEngine.Evaluate("as.matrix(sum.esf$coefficient)").AsNumericMatrix();
                NumericVector vecF = m_pEngine.Evaluate("as.numeric(sum.esf$fstatistic)").AsNumeric();
                CharacterVector vecNames = m_pEngine.Evaluate("attributes(sum.esf$coefficients)$dimnames[[1]]").AsCharacter();
                m_pEngine.Evaluate("fvalue <- as.numeric(sum.esf$fstatistic)");
                double dblPValueF = m_pEngine.Evaluate("pf(fvalue[1],fvalue[2],fvalue[3],lower.tail=F)").AsNumeric().First();
                double dblRsqaure = m_pEngine.Evaluate("sum.esf$r.squared").AsNumeric().First();
                double dblAdjRsqaure = m_pEngine.Evaluate("sum.esf$adj.r.squared").AsNumeric().First();
                double dblResiSE = m_pEngine.Evaluate("sum.esf$sigma").AsNumeric().First();
                NumericVector vecResiDF = m_pEngine.Evaluate("sum.esf$df").AsNumeric();

                if (cboAlternative.Text == "Greater")
                    m_pEngine.Evaluate("sample.esf.resi.mc <- lm.morantest(sample.esf, sample.listw, alternative = 'greater', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Less")
                    m_pEngine.Evaluate("sample.esf.resi.mc <- lm.morantest(sample.esf, sample.listw, alternative = 'less', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Two Sided")
                    m_pEngine.Evaluate("sample.esf.resi.mc <- lm.morantest(sample.esf, sample.listw, alternative = 'two.sided', zero.policy=TRUE)");
                else
                    m_pEngine.Evaluate("sample.esf.resi.mc <- lm.morantest(sample.esf, sample.listw, alternative = 'greater', zero.policy=TRUE)");


                double dblResiMC = m_pEngine.Evaluate("sample.esf.resi.mc$estimate").AsNumeric().First();
                double dblResiMCpVal = m_pEngine.Evaluate("sample.esf.resi.mc$p.value").AsNumeric().First();

                if (cboAlternative.Text == "Greater")
                    m_pEngine.Evaluate("sample.lm.resi.mc <- lm.morantest(sample.lm, sample.listw, alternative = 'greater', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Less")
                    m_pEngine.Evaluate("sample.lm.resi.mc <- lm.morantest(sample.lm, sample.listw, alternative = 'less', zero.policy=TRUE)");
                else if (cboAlternative.Text == "Two Sided")
                    m_pEngine.Evaluate("sample.lm.resi.mc <- lm.morantest(sample.lm, sample.listw, alternative = 'two.sided', zero.policy=TRUE)");
                else
                    m_pEngine.Evaluate("sample.lm.resi.mc <- lm.morantest(sample.lm, sample.listw, alternative = 'greater', zero.policy=TRUE)");

                double dblResiLMMC = m_pEngine.Evaluate("sample.lm.resi.mc$estimate").AsNumeric().First();
                double dblResiLMpVal = m_pEngine.Evaluate("sample.lm.resi.mc$p.value").AsNumeric().First();

                NumericVector nvecNonAIC = m_pEngine.Evaluate("sample.esf$anova$AIC").AsNumeric();

                //Open Ouput form
                frmESFResult pfrmRegResult = new frmESFResult();
                pfrmRegResult.Text = "ESF Linear Regression Summary";

                //Create DataTable to store Result
                System.Data.DataTable tblRegResult = new DataTable("ESFResult");

                //Assign DataTable
                DataColumn dColName = new DataColumn();
                dColName.DataType = System.Type.GetType("System.String");
                dColName.ColumnName = "Name";
                tblRegResult.Columns.Add(dColName);

                DataColumn dColValue = new DataColumn();
                dColValue.DataType = System.Type.GetType("System.Double");
                dColValue.ColumnName = "Estimate";
                tblRegResult.Columns.Add(dColValue);

                DataColumn dColSE = new DataColumn();
                dColSE.DataType = System.Type.GetType("System.Double");
                dColSE.ColumnName = "Std. Error";
                tblRegResult.Columns.Add(dColSE);

                DataColumn dColTValue = new DataColumn();
                dColTValue.DataType = System.Type.GetType("System.Double");
                dColTValue.ColumnName = "t value";
                tblRegResult.Columns.Add(dColTValue);

                DataColumn dColPvT = new DataColumn();
                dColPvT.DataType = System.Type.GetType("System.Double");
                dColPvT.ColumnName = "Pr(>|t|)";
                tblRegResult.Columns.Add(dColPvT);

                int intNCoeff = 0;
                int intNSelectedEVs = matCoe.RowCount - nIDepen - 1;
                if (intInterceptModel == 1)
                    intNSelectedEVs = matCoe.RowCount - nIDepen;

                if (chkCoeEVs.Checked)
                    intNCoeff = matCoe.RowCount;
                else
                {
                    intNCoeff = matCoe.RowCount - intNSelectedEVs;
                }


                //Store Data Table by R result
                for (int j = 0; j < intNCoeff; j++)
                {
                    DataRow pDataRow = tblRegResult.NewRow();
                    if (j == 0 && intInterceptModel != 1)
                    {
                        pDataRow["Name"] = "(Intercept)";
                    }
                    else
                    {
                        if (chkCoeEVs.Checked)
                            pDataRow["Name"] = vecNames[j];
                        else
                        {
                            if (intInterceptModel == 1)
                                pDataRow["Name"] = independentNames[j];
                            else
                                pDataRow["Name"] = independentNames[j - 1];
                        }
                    }
                    pDataRow["Estimate"] = Math.Round(matCoe[j, 0], intDeciPlaces);
                    pDataRow["Std. Error"] = Math.Round(matCoe[j, 1], intDeciPlaces);
                    pDataRow["t value"] = Math.Round(matCoe[j, 2], intDeciPlaces);
                    pDataRow["Pr(>|t|)"] = Math.Round(matCoe[j, 3], intDeciPlaces);
                    tblRegResult.Rows.Add(pDataRow);
                }

                //Assign Datagridview to Data Table
                pfrmRegResult.dgvResults.DataSource = tblRegResult;

                int nFeature = pFLayer.FeatureClass.FeatureCount(null);

                //Get EV and SA for selected filters
                NumericMatrix nmModel = m_pEngine.Evaluate("as.matrix(sample.esf$model)").AsNumericMatrix();
                double[] arrFilter = new double[nFeature];

                //Get individual the selected EVs (v 1.0.6)
                Double[,] arrSelectedEVs = new double[nFeature, intNSelectedEVs];
                String[] arrSelectedEvsNM = null;
                if (intInterceptModel == 1)
                    arrSelectedEvsNM = vecNames.Skip(nIDepen).ToArray();
                else
                    arrSelectedEvsNM = vecNames.Skip(nIDepen + 1).ToArray();


                IFeatureCursor pFCursor = pFLayer.FeatureClass.Search(null, false);
                IFeature pFeature = pFCursor.NextFeature();

                int featureIdx = 0;

                while (pFeature != null)
                {
                    //Calculate and update spatial filter (Coefficient estimate * selected EVs)
                    double dblIntMedValue = 0;
                    double dblFilterValue = 0;
                    for (int k = 0; k < intNSelectedEVs; k++)
                    {
                        if (intInterceptModel == 1)
                        {
                            arrSelectedEVs[featureIdx, k] = nmModel[featureIdx, nIDepen + k]; //Get individual the selected EVs (v 1.0.6)
                            dblIntMedValue = matCoe[nIDepen + k, 0] * nmModel[featureIdx, nIDepen + k];
                        }
                        else
                        {
                            arrSelectedEVs[featureIdx, k] = nmModel[featureIdx, nIDepen + k + 1];//Get individual the selected EVs (v 1.0.6)
                            dblIntMedValue = matCoe[nIDepen + k + 1, 0] * nmModel[featureIdx, nIDepen + k + 1];
                        }

                        dblFilterValue += dblIntMedValue;
                    }

                    arrFilter[featureIdx] = dblFilterValue;
                    pFeature = pFCursor.NextFeature();
                    featureIdx++;
                }

                //Assign the EV and their names into the result form (v1.0.6)
                pfrmRegResult.arrSelectedEVs = arrSelectedEVs;
                pfrmRegResult.arrSelectedEvsNM = arrSelectedEvsNM;
                pfrmRegResult.m_pFClass = m_pFClass;


                //Calculate MC for the constructed ESF
                NumericVector vecFilter = null;
                double dblSfilterMC = 0;
                double dblSfilterpVal = 0;

                if (intNSelectedEVs > 0)
                {
                    vecFilter = m_pEngine.CreateNumericVector(arrFilter);
                    m_pEngine.SetSymbol("samplefilter", vecFilter);

                    if (cboAlternative.Text == "Greater")
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'greater', zero.policy=TRUE)");
                    else if (cboAlternative.Text == "Less")
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'less', zero.policy=TRUE)");
                    else if (cboAlternative.Text == "Two Sided")
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'two.sided', zero.policy=TRUE)");
                    else
                        m_pEngine.Evaluate("sfilter.mc <- moran.test(samplefilter, sample.listw, alternative = 'greater', zero.policy=TRUE)");

                    dblSfilterMC = m_pEngine.Evaluate("sfilter.mc$estimate").AsNumeric().First();
                    dblSfilterpVal = m_pEngine.Evaluate("sfilter.mc$p.value").AsNumeric().First();
                }

                //Assign values at Textbox
                string strDecimalPlaces = "N" + intDeciPlaces.ToString();
                string[] strResults = new string[8];
                strResults[0] = "Number of rows: " + nFeature.ToString() + ", Number of candidate EVs: " + dblNCandidateEvs.ToString() + ", Selected EVs: " + intNSelectedEVs.ToString();

                if (dblResiLMpVal < 0.001)
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value < 0.001";
                else if (dblResiLMpVal > 0.999)
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value > 0.999";
                else
                    strResults[1] = "MC of non-ESF residuals: " + dblResiLMMC.ToString("N3") + ", p-value: " + dblResiLMpVal.ToString("N3");

                strResults[2] = "AIC of non-ESF: " + nvecNonAIC.First().ToString(strDecimalPlaces) + ", AIC of Final Model: " + nvecNonAIC.Last().ToString(strDecimalPlaces);
                //strResults[2] = "AIC of non-ESF: " + m_pEngine.Evaluate("AIC(sample.lm)").AsNumeric().First().ToString(strDecimalPlaces) + ", AIC of Final Model: " + m_pEngine.Evaluate("AIC(sample.esf)").AsNumeric().First().ToString(strDecimalPlaces);

                strResults[3] = "Residual standard error: " + dblResiSE.ToString(strDecimalPlaces) +
                    " on " + vecResiDF[1].ToString() + " degrees of freedom";
                strResults[4] = "Multiple R-squared: " + dblRsqaure.ToString(strDecimalPlaces) +
                    ", Adjusted R-squared: " + dblAdjRsqaure.ToString(strDecimalPlaces);
                if (dblPValueF < 0.001)
                    strResults[5] = "F-Statistic: " + vecF[0].ToString(strDecimalPlaces) +
                    " on " + vecF[1].ToString() + " and " + vecF[2].ToString() + " DF, p-value < 0.001";
                else if (dblPValueF > 0.999)
                    strResults[5] = "F-Statistic: " + vecF[0].ToString(strDecimalPlaces) +
                    " on " + vecF[1].ToString() + " and " + vecF[2].ToString() + " DF, p-value > 0.999";
                else
                    strResults[5] = "F-Statistic: " + vecF[0].ToString(strDecimalPlaces) +
                     " on " + vecF[1].ToString() + " and " + vecF[2].ToString() + " DF, p-value: " + dblPValueF.ToString(strDecimalPlaces);

                if (dblResiMCpVal < 0.001)
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value < 0.001";
                else if (dblResiMCpVal > 0.999)
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value > 0.999";
                else
                    strResults[6] = "MC of residuals: " + dblResiMC.ToString("N3") + ", p-value: " + dblResiMCpVal.ToString("N3");

                if (intNSelectedEVs > 0)
                {
                    if (dblSfilterpVal < 0.001)
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value < 0.001";
                    else if (dblSfilterpVal > 0.999)
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value > 0.999";
                    else
                        strResults[7] = "MC for the constructed ESF: " + dblSfilterMC.ToString(strDecimalPlaces) + ", p-value: " + dblSfilterpVal.ToString(strDecimalPlaces);
                }
                else
                    strResults[7] = "";

                pfrmRegResult.txtOutput.Lines = strResults;

                //Save Outputs in SHP
                if (chkSave.Checked)
                {
                    pfrmProgress.lblStatus.Text = "Saving residuals and spatial filter:";
                    //The field names are related with string[] DeterminedName in clsSnippet 
                    string strResiFldName = lstSave.Items[0].SubItems[1].Text;
                    string strSFilterName = lstSave.Items[1].SubItems[1].Text;

                    NumericVector nvResiduals = m_pEngine.Evaluate("as.numeric(sample.esf$residuals)").AsNumeric();

                    // Create field, if there isn't
                    if (m_pFClass.FindField(strResiFldName) == -1)
                    {
                        //Add fields
                        IField newField = new FieldClass();
                        IFieldEdit fieldEdit = (IFieldEdit)newField;
                        fieldEdit.Name_2 = strResiFldName;
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        m_pFClass.AddField(newField);
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Do you want to overwrite " + strResiFldName + " field?", "Overwrite", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (m_pFClass.FindField(strSFilterName) == -1)
                    {
                        IField newField = new FieldClass();
                        IFieldEdit fieldEdit = (IFieldEdit)newField;
                        fieldEdit.Name_2 = strSFilterName;
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                        m_pFClass.AddField(newField);
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Do you want to overwrite " + strSFilterName + " field?", "Overwrite", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.No)
                        {
                            return;
                        }
                    }

                    //Update Field
                    pFCursor = pFLayer.FeatureClass.Update(null, false);
                    pFeature = pFCursor.NextFeature();

                    featureIdx = 0;
                    int intResiFldIdx = m_pFClass.FindField(strResiFldName);
                    int intSFilterIdx = m_pFClass.FindField(strSFilterName);

                    while (pFeature != null)
                    {
                        //Update Residuals
                        pFeature.set_Value(intResiFldIdx, (object)nvResiduals[featureIdx]);
                        if (intNSelectedEVs > 0)
                            pFeature.set_Value(intSFilterIdx, (object)vecFilter[featureIdx]);

                        pFCursor.UpdateFeature(pFeature);

                        pFeature = pFCursor.NextFeature();
                        featureIdx++;
                    }

                }

                pfrmRegResult.Show();
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog(); pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                pfrmProgress.Close();
                return;
            }
        }
        
        
        private void lstFields_DoubleClick(object sender, EventArgs e)
        {
            m_pSnippet.MoveSelectedItemsinListBoxtoOtherListBox(lstFields, lstIndeVar);
        }

        private void lstIndeVar_DoubleClick(object sender, EventArgs e)
        {
            m_pSnippet.MoveSelectedItemsinListBoxtoOtherListBox(lstIndeVar, lstFields);
        }

        private void chkSave_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSave.Checked)
            {
                if (m_pFClass != null)
                {
                    UpdateListview(lstSave, m_pFClass);
                    lstSave.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Select a target layer first");

                    chkSave.Checked = false;
                    return;
                }
            }
            else
                lstSave.Enabled = false;
        }
        private void UpdateListview(ListView pListView, IFeatureClass pFeatureClass)
        {
            try
            {
                pListView.BeginUpdate();
                string strResiFldNm = "esf_resi";
                string strSFilterNm = "sfilter";
                
                //Update Name Using the UpdateFldName Function

                strResiFldNm = UpdateFldName(strResiFldNm, pFeatureClass);
                strSFilterNm = UpdateFldName(strSFilterNm, pFeatureClass);

                if (strResiFldNm == null || strSFilterNm == null )
                    return;

                pListView.Items[0].SubItems[1].Text = strResiFldNm;
                pListView.Items[1].SubItems[1].Text = strSFilterNm;
                pListView.EndUpdate();
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog();pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                return;
            }

        }
        private string UpdateFldName(string strFldNM, IFeatureClass pFeatureClass)
        {
            try
            {
                string returnNM = strFldNM;
                int i = 1;
                while (pFeatureClass.FindField(returnNM) != -1)
                {
                    returnNM = strFldNM + "_" + i.ToString();
                    i++;
                }
                return returnNM;
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog();pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                return null;
            }
        }

        private DialogResult PopupInput(ListViewItem.ListViewSubItem pSelectedSubItems, int border, int length, ref string output)
        {

            System.Drawing.Point ctrlpt = pSelectedSubItems.Bounds.Location;
            ctrlpt = this.PointToScreen(pSelectedSubItems.Bounds.Location);
            //ctrlpt.Y += grbSave.Location.Y + 10;
            //ctrlpt.X += grbSave.Location.X + (length / 2);
            ctrlpt.Y += 263;
            ctrlpt.X += 308 + (length / 2);
            TextBox input = new TextBox { Height = 20, Width = length, Top = border / 2, Left = border / 2 };
            input.BorderStyle = BorderStyle.FixedSingle;
            input.Text = output;
            //######## SetColor to your preference
            input.BackColor = Color.Azure;

            Button btnok = new Button { DialogResult = System.Windows.Forms.DialogResult.OK, Top = 25 };
            Button btncn = new Button { DialogResult = System.Windows.Forms.DialogResult.Cancel, Top = 25 };

            Form frm = new Form { ControlBox = false, AcceptButton = btnok, CancelButton = btncn, StartPosition = FormStartPosition.Manual, Location = ctrlpt };
            frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //######## SetColor to your preference
            frm.BackColor = Color.Black;

            RectangleF rec = new RectangleF(0, 0, (length + border), (20 + border));
            System.Drawing.Drawing2D.GraphicsPath GP = new System.Drawing.Drawing2D.GraphicsPath(); //GetRoundedRect(rec, 4.0F);
            float diameter = 8.0F;
            SizeF sizef = new SizeF(diameter, diameter);
            RectangleF arc = new RectangleF(rec.Location, sizef);
            GP.AddArc(arc, 180, 90);
            arc.X = rec.Right - diameter;
            GP.AddArc(arc, 270, 90);
            arc.Y = rec.Bottom - diameter;
            GP.AddArc(arc, 0, 90);
            arc.X = rec.Left;
            GP.AddArc(arc, 90, 90);
            GP.CloseFigure();

            frm.Region = new Region(GP);
            frm.Controls.AddRange(new Control[] { input, btncn, btnok });
            DialogResult rst = frm.ShowDialog();
            output = input.Text;
            return rst;
        }

        private void lstSave_MouseUp(object sender, MouseEventArgs e)
        {
            ListViewItem.ListViewSubItem pSelectedSubItems = null;
            //selection = lvSymbol.GetItemAt(e.X, e.Y);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    if (lstSave.Items[i].SubItems[j].Bounds.Contains(e.Location))
                    {
                        pSelectedSubItems = lstSave.Items[i].SubItems[j];
                        if (j == 1)
                        {
                            string var = pSelectedSubItems.Text;

                            int intLength = var.Length * 6 + 30;

                            if (PopupInput(pSelectedSubItems, 2, intLength, ref var) == System.Windows.Forms.DialogResult.OK)
                            {
                                lstSave.Items[i].SubItems[j].Text = var;
                            }
                        }

                    }
                }
            }

            lstSave.Update();
        }

        private void cboFamily_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTargetLayer.Text != "" && cboFamily.Text != "")
                {
                    if (cboFamily.Text == "Linear (Gaussian)"|| cboFamily.Text == "Logistic")
                    {
                        cboNormalization.Enabled = false;
                        cboNormalization.Text = "";
                        lblNorm.Enabled = false;
                    }
                    else
                    {
                        cboNormalization.Enabled = true;
                        lblNorm.Enabled = true;
                        if (cboFamily.Text == "Binomial")
                            lblNorm.Text = "Normalization";
                        else
                            lblNorm.Text = "Offset";
                    }

                    cboAlternative.Items.Clear();

                    if(cboFamily.Text == "Linear (Gaussian)")
                    {
                        cboAlternative.Items.Add("Greater");
                        cboAlternative.Items.Add("Less");
                        cboAlternative.Items.Add("Two Sided");
                    }
                    else
                    {
                        cboAlternative.Items.Add("Greater");
                        cboAlternative.Items.Add("Less");
                        cboAlternative.Text = "Greater";
                    }
                        
                    string strLayerName = cboTargetLayer.Text;

                    int intLIndex = m_pSnippet.GetIndexNumberFromLayerName(m_pActiveView, strLayerName);
                    ILayer pLayer = m_pForm.axMapControl1.get_Layer(intLIndex);

                    m_pFLayer = pLayer as IFeatureLayer;
                    m_pFClass = m_pFLayer.FeatureClass;

                    IFields fields = m_pFClass.Fields;

                    cboFieldName.Items.Clear();
                    cboFieldName.Text = "";
                    lstFields.Items.Clear();
                    lstIndeVar.Items.Clear();
                    cboNormalization.Text = "";
                    cboNormalization.Items.Clear();

                    if (cboFamily.Text == "Linear (Gaussian)")
                        rbEquation.Enabled = true;
                    else
                    {
                        rbEquation.Enabled = false;
                        rbEValue.Checked = true;
                    }

                    if (cboFamily.Text == "Poisson"|| cboFamily.Text == "Logistic" || cboFamily.Text == "Negative Binomial")
                    {
                        for (int i = 0; i < fields.FieldCount; i++)
                        {
                            if (m_pSnippet.FindNumberFieldType(fields.get_Field(i)))
                            {
                                lstFields.Items.Add(fields.get_Field(i).Name);
                                cboNormalization.Items.Add(fields.get_Field(i).Name);
                                if (fields.get_Field(i).Type == esriFieldType.esriFieldTypeInteger || fields.get_Field(i).Type == esriFieldType.esriFieldTypeSmallInteger)
                                    cboFieldName.Items.Add(fields.get_Field(i).Name);
                            }

                        }
                    }
                    else
                    {
                        for (int i = 0; i < fields.FieldCount; i++)
                        {
                            if (m_pSnippet.FindNumberFieldType(fields.get_Field(i)))
                            {
                                cboFieldName.Items.Add(fields.get_Field(i).Name);
                                lstFields.Items.Add(fields.get_Field(i).Name);
                                cboNormalization.Items.Add(fields.get_Field(i).Name);
                            }
                        }
                    }

                    //Add intercept in the listview for independent variables
                    lstIndeVar.Items.Add("Intercept");

                    //Save Result
                    if (chkSave.Checked)
                        UpdateListview(lstSave, m_pFClass);

                }
            }
            catch (Exception ex)
            {
                frmErrorLog pfrmErrorLog = new frmErrorLog();pfrmErrorLog.ex = ex; pfrmErrorLog.ShowDialog();
                return;
            }
        }

        private void rbEValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEValue.Checked)
            {
                nudEValue.Enabled = true;
                cboDirection.Enabled = true;
                rbEquation.Checked = false;
                lblDirection.Enabled = true;
            }

        }
        private void rbEquation_CheckedChanged(object sender, EventArgs e)
        {
            if (rbEquation.Checked)
            {
                cboDirection.Enabled = false;
                rbEValue.Checked = false;
                nudEValue.Enabled = false;
                lblDirection.Enabled = false;
            }

        }

        private void btnOpenSWM_Click(object sender, EventArgs e)
        {
            if (m_pFClass == null)
            {
                MessageBox.Show("Please select a target layer");
                return;
            }
            frmAdvSWM pfrmAdvSWM = new frmAdvSWM();
            pfrmAdvSWM.m_pFClass = m_pFClass;
            pfrmAdvSWM.blnCorrelogram = false;
            pfrmAdvSWM.ShowDialog();
            m_blnCreateSWM = pfrmAdvSWM.blnSWMCreation;
            txtSWM.Text = pfrmAdvSWM.txtSWM.Text;
        }

        private void cboNormalization_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboNormalization.Text == "offset")
            {
                MessageBox.Show("The field name of 'offset' cannot be used for an offset variable name in this tool. Please assign the field to another name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboNormalization.Text = "";
            }

            if (cboNormalization.Text == "area")
            {
                MessageBox.Show("The field name of 'area' cannot be used for an offset variable name in this tool. Please assign the field to another name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboNormalization.Text = "";
            }
        }

        //private void chkIntercept_CheckedChanged(object sender, EventArgs e)
        //{
        //    if(chkIntercept.Checked == false)
        //    {
        //        lstFields.Enabled = true;
        //        lstIndeVar.Enabled = true;
        //        btnMoveLeft.Enabled = true;
        //        btnMoveRight.Enabled = true;
        //    }
        //    else
        //    {
        //        lstFields.Enabled = false;
        //        lstIndeVar.Enabled = false;
        //        btnMoveLeft.Enabled = false;
        //        btnMoveRight.Enabled = false;
        //    }
        //}
    }
}
