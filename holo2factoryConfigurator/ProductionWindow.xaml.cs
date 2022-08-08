using HelixToolkit.Wpf;
using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.IO;

namespace Holo2factoryConfigurator
{
    public partial class ProductionWindow : Window
    {
        private new bool IsInitialized { get; set; } = false;
        private string DirectoryPath { get; set; } = Directory.GetCurrentDirectory();
        private const float CaliperPrecision = 0.01f;
        private static (double, double, double, double, double, double) ModelOffset { get; set; } = default;


        public ProductionWindow(string ModelPath)
        {
            InitializeComponent();
            IsInitialized = true;
            //window culture
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

            ObjReader CurrentHelixObjReader = new ObjReader();
            try
            {
                //load the 3D Model with offset if set
                Model3DGroup Model3DGroup = CurrentHelixObjReader.Read(ModelPath);
                Transform3DGroup transform3DGroup = new Transform3DGroup();
                RotateTransform3D rotateTransform3DX =
                    new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), ModelOffset.Item4));
                RotateTransform3D rotateTransform3DY =
                    new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), ModelOffset.Item5));
                RotateTransform3D rotateTransform3DZ =
                    new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), ModelOffset.Item6));
                transform3DGroup.Children.Add(rotateTransform3DX);
                transform3DGroup.Children.Add(rotateTransform3DY);
                transform3DGroup.Children.Add(rotateTransform3DZ);
                transform3DGroup.Children.Add(new TranslateTransform3D(new Vector3D(
                    ModelOffset.Item1, ModelOffset.Item2, ModelOffset.Item3)));
                myModel.Transform = transform3DGroup;
                myModel.Content = Model3DGroup;
                //rewrite adjustments values
                TextBoxAdjX.Text = ModelOffset.Item1.ToString();
                TextBoxAdjY.Text = ModelOffset.Item2.ToString();
                TextBoxAdjZ.Text = ModelOffset.Item3.ToString();
                TextBoxAdjRx.Text = ModelOffset.Item4.ToString();
                TextBoxAdjRy.Text = ModelOffset.Item5.ToString();
                TextBoxAdjRz.Text = ModelOffset.Item6.ToString();
            }
            catch (Exception)
            {
                _ = MessageBox.Show("Unable to load the model, please check your file", "Load 3D model", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public bool Execute(PieceCotation newCotation)
        {
            Owner = Application.Current.MainWindow;
            LabelCotationNb.Content = "Cotation ID: " + newCotation.Id.ToString();
            TextBoxX.Text = newCotation.X.ToString();
            TextBoxY.Text = newCotation.Y.ToString();
            TextBoxZ.Text = newCotation.Z.ToString();
            TextBoxRx.Text = newCotation.Rx.ToString();
            TextBoxRy.Text = newCotation.Ry.ToString();
            TextBoxRz.Text = newCotation.Rz.ToString();
            TextBoxExpected.Text = newCotation.ExpectedValue.ToString();
            TextBoxTolerance.Text = newCotation.Tolerance.ToString();
            ComboboxType.SelectedItem = newCotation.Type.ToString();
            bool response = ShowDialog() == true;
            if (response)
            {
                if (ComboboxType.Text == "Rectilign")
                {
                    newCotation.Type = PieceCotation.CotationType.Rectilign;
                }
                else if (ComboboxType.Text == "Diameter")
                {
                    newCotation.Type = PieceCotation.CotationType.Diameter;
                }
                else
                {
                    newCotation.Type = PieceCotation.CotationType.Hole;
                }
                // check values
                try
                {
                    newCotation.X = float.Parse(TextBoxX.Text);
                    newCotation.Y = float.Parse(TextBoxY.Text);
                    newCotation.Z = float.Parse(TextBoxZ.Text);
                    newCotation.Rx = float.Parse(TextBoxRx.Text);
                    newCotation.Ry = float.Parse(TextBoxRy.Text);
                    newCotation.Rz = float.Parse(TextBoxRz.Text);
                    newCotation.ExpectedValue = float.Parse(TextBoxExpected.Text);
                    newCotation.Tolerance = float.Parse(TextBoxTolerance.Text);
                }
                catch (FormatException)
                {
                    _ = MessageBox.Show("Float format incorrect, check your values", "Format error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return response;
        }
        //IsCancel is set in xaml
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                float.Parse(TextBoxX.Text);
                float.Parse(TextBoxY.Text);
                float.Parse(TextBoxZ.Text);
                float.Parse(TextBoxRx.Text);
                float.Parse(TextBoxRy.Text);
                float.Parse(TextBoxRz.Text);
                float expect = float.Parse(TextBoxExpected.Text);
                float tol = float.Parse(TextBoxTolerance.Text);
                if (expect <= 0)
                {
                    throw new ArgumentOutOfRangeException("Expected value has to be greater than 0");

                }
                else if (tol < CaliperPrecision)
                {
                    throw new ArgumentOutOfRangeException("Tolerance is under the precision of the caliper");
                }
                else
                {
                    DialogResult = true;
                }
            }
            catch (ArgumentOutOfRangeException exc)
            {
                _ = MessageBox.Show(exc.Message, "Range error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (FormatException)
            {
                _ = MessageBox.Show("Float format incorrect, check your values", "Format error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }
        private void AdjustModel()
        {
            if (!IsInitialized)
                return;
            const double Ratio = 10; //Helix uses cm instead of mm
            double x, y, z, rx, ry, rz;
            try
            {
                x = double.Parse(TextBoxAdjX.Text.Replace(',', '.')) / Ratio;
                y = double.Parse(TextBoxAdjY.Text.Replace(',', '.')) / Ratio;
                z = double.Parse(TextBoxAdjZ.Text.Replace(',', '.')) / Ratio;
                rx = double.Parse(TextBoxAdjRx.Text.Replace(',', '.'));
                ry = double.Parse(TextBoxAdjRy.Text.Replace(',', '.'));
                rz = double.Parse(TextBoxAdjRz.Text.Replace(',', '.'));
                ModelOffset = (x, y, z, rx, ry, rz);
            }
            catch (FormatException)
            {
                return;
            }
            //set transformation to the model with the parameters
            Transform3DGroup transform3DGroup = new Transform3DGroup();
            RotateTransform3D rotateTransform3DX = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), rx));
            RotateTransform3D rotateTransform3DY = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), ry));
            RotateTransform3D rotateTransform3DZ = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), rz));
            transform3DGroup.Children.Add(rotateTransform3DX);
            transform3DGroup.Children.Add(rotateTransform3DY);
            transform3DGroup.Children.Add(rotateTransform3DZ);
            transform3DGroup.Children.Add(new TranslateTransform3D(new Vector3D(x, y, z)));
            //apply the transformation
            myModel.Transform = transform3DGroup;

        }
        private void TextBoxAdjX_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustModel();
        }

        private void TextBoxAdjY_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustModel();
        }

        private void TextBoxAdjZ_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustModel();
        }

        private void TextBoxAdjRx_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustModel();
        }

        private void TextBoxAdjRy_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustModel();
        }

        private void TextBoxAdjRz_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustModel();
        }

        private void AdjustCotation()
        {
            if (!IsInitialized)
                return;
            const double Ratio = 10; //Helix uses cm instead of mm
            const double minScale = 0.5;
            const double maxScale = 3;
            ObjReader CurrentHelixObjReader = new ObjReader();
            Model3DGroup CotationGroup;
            double x, y, z, rx, ry, rz, length;
            try
            {
                x = double.Parse(TextBoxX.Text.Replace(',', '.')) / Ratio;
                y = double.Parse(TextBoxY.Text.Replace(',', '.')) / Ratio;
                z = double.Parse(TextBoxZ.Text.Replace(',', '.')) / Ratio;
                rx = double.Parse(TextBoxRx.Text.Replace(',', '.'));
                ry = double.Parse(TextBoxRy.Text.Replace(',', '.'));
                rz = double.Parse(TextBoxRz.Text.Replace(',', '.'));
                length = double.Parse(TextBoxExpected.Text.Replace(',', '.')) / Ratio;

            }
            catch (FormatException)
            {
                return;
            }
            catch (System.NullReferenceException)
            {
                return;
            }
            ScaleTransform3D scaleTransform3D;
            //Instantiate the correct cotation upon selected type
            if (ComboboxType.Text == "Rectilign")
            {
                CotationGroup = CurrentHelixObjReader.Read(DirectoryPath + "/CotationsModels/CotationArrow.obj");
                double scaleRatio = length < minScale ? minScale : length > maxScale ? maxScale : length;
                scaleTransform3D = new ScaleTransform3D(scaleRatio, scaleRatio, scaleRatio, x, y, z);
            }
            else if (ComboboxType.Text == "Diameter")
            {
                CotationGroup = CurrentHelixObjReader.Read(DirectoryPath + "/CotationsModels/CotationDiameter.obj");
                scaleTransform3D = new ScaleTransform3D(length, length, 1, x, y, z);
            }
            else
            {
                CotationGroup = CurrentHelixObjReader.Read(DirectoryPath + "/CotationsModels/CotationHole.obj");
                double scaleRatio = length < minScale ? minScale : length > maxScale ? maxScale : length;
                scaleTransform3D = new ScaleTransform3D(scaleRatio, scaleRatio, scaleRatio, x, y, z);
            }
            //set transformation to the cotation with the parameters upon the cotation type
            Transform3DGroup transform3DGroup = new Transform3DGroup();
            RotateTransform3D rotateTransform3DX = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), rx));
            RotateTransform3D rotateTransform3DY = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), ry));
            RotateTransform3D rotateTransform3DZ = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), rz));
            transform3DGroup.Children.Add(rotateTransform3DX);
            transform3DGroup.Children.Add(rotateTransform3DY);
            transform3DGroup.Children.Add(rotateTransform3DZ);
            transform3DGroup.Children.Add(new TranslateTransform3D(new Vector3D(x, y, z)));
            transform3DGroup.Children.Add(scaleTransform3D);
            CotationGroup.Transform = transform3DGroup;
            cotationModel.Content = CotationGroup;
        }
        //display correct cotation symbol upon cotation type

        private void TextBoxX_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustCotation();
        }

        private void TextBoxY_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustCotation();
        }

        private void TextBoxZ_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustCotation();
        }

        private void TextBoxRx_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustCotation();
        }

        private void TextBoxRy_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustCotation();
        }

        private void TextBoxRz_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustCotation();
        }

        private void ComboboxType_DropDownClosed(object sender, EventArgs e)
        {
            AdjustCotation();
        }

        private void TextBoxExpected_TextChanged(object sender, TextChangedEventArgs e)
        {
            AdjustCotation();
        }
    }
}
