namespace Talent.RemoteCarMeasure.Report
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;
    using System.Collections.Generic;
    using Talent.RemoteCarMeasure.Model;

    /// <summary>
    /// Summary description for ReportTest.
    /// </summary>
    public partial class ReportTest : Telerik.Reporting.Report
    {
        public ReportTest()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();


            //ProductCollection  coll=new ProductCollection();
            // ReportProductModel model=new ReportProductModel();
            //model.ProductCode="1";
            //model.Code="2";
            //model.Name="3";
            //coll.Add(model);
            // ReportProductModel model1=new ReportProductModel();
            //model1.ProductCode="1";
            //model1.Code="4";
            //model1.Name="5";
            //coll.Add(model1);
            //this.objectDataSource1.DataSource = coll;
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public void SetCrossTableDataSource(List<WeightRecordModel> WeightRecordDataList)
        {
            this.crosstab2.DataSource = WeightRecordDataList;
        }
    }
}