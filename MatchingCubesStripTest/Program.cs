using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kitware.VTK;

namespace MatchingCubesStripTest
{
    class Program
    {
        static void Main(string[] args)
        {
            MatchCubeStripTest1();
        }

        public static void MatchCubeStripTest1()
        {
            vtkDICOMImageReader dcmReader = vtkDICOMImageReader.New();
            dcmReader.SetDirectoryName("../../../../res/CT");
            dcmReader.Update();

            vtkMarchingCubes pMatchingCube = vtkMarchingCubes.New();
            pMatchingCube.SetInputConnection(dcmReader.GetOutputPort());
            pMatchingCube.SetValue(0, -500); // iso value
            pMatchingCube.ComputeScalarsOff();
            pMatchingCube.ComputeNormalsOn();
            pMatchingCube.Update();

            vtkStripper skinStripper = vtkStripper.New();
            skinStripper.SetInputConnection(pMatchingCube.GetOutputPort());

            vtkPolyDataMapper mapper = vtkPolyDataMapper.New();
            mapper.SetInputConnection(skinStripper.GetOutputPort());
            mapper.ScalarVisibilityOff();


            vtkMarchingCubes pMatchingCube2 = vtkMarchingCubes.New();
            pMatchingCube2.SetInputConnection(dcmReader.GetOutputPort());
            pMatchingCube2.SetValue(0, 330); // iso value  333
            pMatchingCube2.ComputeScalarsOff();
            pMatchingCube2.ComputeNormalsOn();
            pMatchingCube2.Update();

            vtkStripper boneStripper2 = vtkStripper.New();
            boneStripper2.SetInputConnection(pMatchingCube2.GetOutputPort());

            vtkPolyDataMapper mapper2 = vtkPolyDataMapper.New();
            mapper2.SetInputConnection(boneStripper2.GetOutputPort());
            mapper2.ScalarVisibilityOff();

            vtkActor actor = vtkActor.New();
            actor.SetMapper(mapper);
            actor.GetProperty().SetDiffuseColor(1, 1, 1); // (.1, .94, .52);
            actor.GetProperty().SetSpecular(.3);
            actor.GetProperty().SetSpecularPower(20);
            actor.GetProperty().SetOpacity(0.5);

            vtkActor actor2 = vtkActor.New();
            actor2.SetMapper(mapper2);

            vtkCamera aCamera = vtkCamera.New();
            aCamera.SetViewUp(0, 0, -1);
            aCamera.SetPosition(0, 1, 0);
            aCamera.SetFocalPoint(0, 0, 0);

            vtkRenderer renderer = vtkRenderer.New();
            renderer.AddActor(actor);
            renderer.AddActor(actor2);
            renderer.SetActiveCamera(aCamera);
            renderer.ResetCamera();
            aCamera.Dolly(1.5);
            renderer.SetBackground(.1, .2, .3);
            renderer.ResetCameraClippingRange();

            vtkRenderWindow renderWin = vtkRenderWindow.New();
            renderWin.AddRenderer(renderer);

            vtkRenderWindowInteractor interactor = vtkRenderWindowInteractor.New();
            interactor.SetRenderWindow(renderWin);

            renderWin.Render();
            interactor.Start();

        }
    }
}
