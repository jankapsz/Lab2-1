using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Szeminarium;

namespace GrafikaSzeminarium
{
    internal class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static ModelObjectDescriptor cube;
        private static ModelObjectDescriptor[] cubes = new ModelObjectDescriptor[26];

        private static CameraDescriptor camera = new CameraDescriptor();

        private static CubeArrangementModel cubeArrangementModel = new CubeArrangementModel();

        private const string ModelMatrixVariableName = "uModel";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;

		out vec4 outCol;
        
        void main()
        {
			outCol = vCol;
            gl_Position = uProjection*uView*uModel*vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";


        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
		
		in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
        ";

        private static float[][] coords = new float[][]
        {
            // ELULSO LAP
            // 0 - bal felso elol
            new float[] {-1.1f, 1.1f, 1.1f},
            // 1 - kozepso felso elol
            new float[] {0.0f, 1.1f, 1.1f},
            // 2 - jobb felso elol
            new float[] {1.1f, 1.1f, 1.1f},
            // 3 - bal kozepso elol
            new float[] {-1.1f, 0.0f, 1.1f},
            // 4 - kozepso kozepso elol
            new float[] {0.0f, 0.0f, 1.1f},
            // 5 - jobb kozepso elol
            new float[] {1.1f, 0.0f, 1.1f},
            // 6 - bal also elol
            new float[] {-1.1f, -1.1f, 1.1f},
            // 7 - kozepso also elol
            new float[] {0.0f, -1.1f, 1.1f},
            // 8 - jobb also elol
            new float[] {1.1f, -1.1f, 1.1f},

            // KOZEPSO LAP
            // 9 - bal felso kozepen
            new float[] {-1.1f, 1.1f, 0.0f},
            // 10 - kozepso felso kozepen
            new float[] {0.0f, 1.1f, 0.0f},
            // 11 - jobb felso kozepen
            new float[] {1.1f, 1.1f, 0.0f},
            // 12 - jobb kozepso kozepen
            new float[] {1.1f, 0.0f, 0.0f},
            // 13 - jobb also kozepen
            new float[] {1.1f, -1.1f, 0.0f},
            // 14 - kozepso also kozepen
            new float[] {0.0f, -1.1f, 0.0f},
            // 15 - bal also kozepen
            new float[] {-1.1f, -1.1f, 0.0f},
            // 16 - bal kozepso kozepen
            new float[] {-1.1f, 0.0f, 0.0f},

            // HATSO LAP
            // 17 - bal felso hatul
            new float[] {-1.1f, 1.1f, -1.1f},
            // 18 - kozepso felso hatul
            new float[] {0.0f, 1.1f, -1.1f},
            // 19 - jobb felso hatul
            new float[] {1.1f, 1.1f, -1.1f},
            // 20 - jobb kozepso hatul
            new float[] {1.1f, 0.0f, -1.1f},
            // 21 - jobb also hatul
            new float[] {1.1f, -1.1f, -1.1f},
            // 22 - kozepso also hatul
            new float[] {0.0f, -1.1f, -1.1f},
            // 23 - bal also hatul
            new float[] {-1.1f, -1.1f, -1.1f},
            // 24 - bal kozepso hatul
            new float[] {-1.1f, 0.0f, -1.1f},
            // 25 - kozepso kozepso hatul
            new float[] {0.0f, 0.0f, -1.1f},
        };



        private static uint program;

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "Grafika szeminárium";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;
            graphicWindow.Closing += GraphicWindow_Closing;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Closing()
        {
            cube.Dispose();
            Gl.DeleteProgram(program);
        }

        private static void GraphicWindow_Load()
        {
            Gl = graphicWindow.CreateOpenGL();

            var inputContext = graphicWindow.CreateInput();
            foreach (var keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
            }

 ///////////////////////////////////////////////////////////////////////////////////
            cube = ModelObjectDescriptor.CreateCube(Gl);

            for (int i = 0; i < 26; i++)
            {
                cubes[i] = ModelObjectDescriptor.CreateCube(Gl);
            }

            Gl.ClearColor(System.Drawing.Color.White);
            
            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(TriangleFace.Back);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);


            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(fshader);
            Gl.GetShader(fshader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + Gl.GetShaderInfoLog(fshader));

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);

            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);
            if ((ErrorCode)Gl.GetError() != ErrorCode.NoError)
            {

            }

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }
        }

        private static void Keyboard_KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.Left:
                    camera.RotateLeft();
                    break;
                case Key.Right:
                    camera.RotateRight();
                    break;
                case Key.Down:
                    camera.RotateDown();
                    break;
                case Key.Up:
                    camera.RotateUp();
                    break;
                case Key.W:
                    camera.MoveForward();
                    break;
                case Key.A:
                    camera.MoveLeft();
                    break;
                case Key.S:
                    camera.MoveBackward();
                    break;
                case Key.D:
                    camera.MoveRight();
                    break;
                case Key.Space:
                    camera.MoveUp();
                    break;
                case Key.ShiftLeft:
                    camera.MoveDown();
                    break;
            }
        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO OpenGL
            // make it threadsafe
            cubeArrangementModel.AdvanceTime(deltaTime);
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            Gl.UseProgram(program);

            var viewMatrix = Matrix4X4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);
            SetMatrix(viewMatrix, ViewMatrixVariableName);

            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)(Math.PI / 2), 1024f / 768f, 0.1f, 100f);
            SetMatrix(projectionMatrix, ProjectionMatrixVariableName);


            //var modelMatrixCenterCube = Matrix4X4.CreateScale((float)cubeArrangementModel.CenterCubeScale);
            //SetMatrix(modelMatrixCenterCube, ModelMatrixVariableName);
            //DrawModelObject(cube);

            // little cubes drawing
            for(int i=0; i<26; i++)
            {
                var translation = Matrix4X4.CreateTranslation(coords[i][0], coords[i][1], coords[i][2]);
                SetMatrix(translation, ModelMatrixVariableName);
                DrawModelObject(cubes[i]);
            }

        }

        private static unsafe void DrawModelObject(ModelObjectDescriptor modelObject)
        {
            Gl.BindVertexArray(modelObject.Vao);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, modelObject.Indices);
            Gl.DrawElements(PrimitiveType.Triangles, modelObject.IndexArrayLength, DrawElementsType.UnsignedInt, null);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }

        private static unsafe void SetMatrix(Matrix4X4<float> mx, string uniformName)
        {
            int location = Gl.GetUniformLocation(program, uniformName);
            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&mx);
            CheckError();
        }

        public static void CheckError()
        {
            var error = (ErrorCode)Gl.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }
}