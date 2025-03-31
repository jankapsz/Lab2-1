using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GrafikaSzeminarium
{
    internal class ModelObjectDescriptor:IDisposable
    {
        private bool disposedValue;

        public uint Vao { get; private set; }
        public uint Vertices { get; private set; }
        public uint Colors { get; private set; }
        public uint Indices { get; private set; }
        public uint IndexArrayLength { get; private set; }

        private GL Gl;


        private static float[] colors = new float[] {
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                1.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 1.0f,
                1.0f, 0.0f, 1.0f, 1.0f,

                0.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 1.0f, 1.0f,

                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
                1.0f, 1.0f, 0.0f, 1.0f,
            };

        //maszkok
        //1-P, 2-R, 3-G, 4-Y, 5-O, 6-B
        private static float[][] cubeColorMask = new float[][] { 
            //            1-P,  2-R,  3-G,  4-Y,  5-O,  6-B
            //0 Purple-Red-Green
            new float[] { 1.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f },
            //1 Purple-Red
            new float[] { 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
            //2 Purple-Red-Blue
            new float[] { 1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f },
            //3 Red-Green
            new float[] { 0.0f, 1.0f, 1.0f, 0.0f, 0.0f, 0.0f },
            //4 Red
            new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
            //5 Red-Blue
            new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f },
            //6 Red-Green-Yellow
            new float[] { 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, 0.0f },
            //7 Red-Yellow
            new float[] { 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f },
            //8 Red-Blue-Yellow
            new float[] { 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 1.0f },
            //9 Green-Purple
            new float[] { 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
            //10 Purple
            new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
            //11 Purple-Blue
            new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f },
            //12 Blue
            new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f },
            //13 Blue-Yellow
            new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f },
            //14 Yellow
            new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
            //15 Yellow-Green
            new float[] { 0.0f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f },
            //16 Green 
            new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
            //17 Purple-Green-Orange
            new float[] { 1.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f },
            //18 Purple-Orange
            new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
            //19 Purple-Blue-Orange
            new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f },
            //20 Blue-Orange
            new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f },
            //21 Blue-Yellow-Orange
            new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f },
            //22 Yellow-Orange
            new float[] { 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f },
            //23 Yellow-Green-Orange
            new float[] { 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f },
            //24 Green-Orange
            new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f },
            //25 Orange
            new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f },
            //26 Black
            new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f },
            //27 Colorfull
            new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f },

        };


        private static float[] maskedColors(int cubeID)
        {
            float[] maskcolors = (float[])colors.Clone();
            int colorIndex;
            for (int i = 0; i < 6; i++)
            {
                colorIndex = i * 16;
                for(int j=0; j<16; j++)
                {
                    maskcolors[colorIndex + j] *= cubeColorMask[cubeID][i];
                }
            }
            return maskcolors;
        }

        public unsafe static ModelObjectDescriptor CreateCube(GL Gl, int cubeID)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            float[] colorArray = maskedColors(cubeID);

            // counter clockwise is front facing
            var vertexArray = new float[] {
                -0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, -0.5f,

                -0.5f, 0.5f, 0.5f,
                -0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, 0.5f,
                0.5f, 0.5f, 0.5f,

                -0.5f, 0.5f, 0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, 0.5f,

                -0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,

                0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,

                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, -0.5f,
                0.5f, -0.5f, -0.5f,
                0.5f, -0.5f, 0.5f,

            };

           

            uint[] indexArray = new uint[] {
                0, 1, 2,
                0, 2, 3,

                4, 5, 6,
                4, 6, 7,

                8, 9, 10,
                10, 11, 8,

                12, 14, 13,
                12, 15, 14,

                17, 16, 19,
                17, 19, 18,

                20, 22, 21,
                20, 23, 22
            };

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);

            return new ModelObjectDescriptor() {Vao= vao, Vertices = vertices, Colors = colors, Indices = indices, IndexArrayLength = (uint)indexArray.Length, Gl = Gl};

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null


                // always unbound the vertex buffer first, so no halfway results are displayed by accident
                Gl.DeleteBuffer(Vertices);
                Gl.DeleteBuffer(Colors);
                Gl.DeleteBuffer(Indices);
                Gl.DeleteVertexArray(Vao);

                disposedValue = true;
            }
        }

        ~ModelObjectDescriptor()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
