// Autogenerated file. Do not edit by hand
using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
    public class GPUCopy
    {
        ComputeShader m_Shader;
        int k_SampleKernel_xyzw2x_8;
        int k_SampleKernel_xyzw2x_1;

        public GPUCopy(ComputeShader shader)
        {
            m_Shader = shader;
            k_SampleKernel_xyzw2x_8 = m_Shader.FindKernel("KSampleCopy4_1_x_8");
            k_SampleKernel_xyzw2x_1 = m_Shader.FindKernel("KSampleCopy4_1_x_1");
        }

        static readonly int _RectOffset = Shader.PropertyToID("_RectOffset");
        static readonly int _Result1 = Shader.PropertyToID("_Result1");
        static readonly int _Source4 = Shader.PropertyToID("_Source4");
        void SampleCopyChannel(
            CommandBuffer cmd,
            RectInt rect,
            int _source,
            RenderTargetIdentifier source,
            int _target,
            RenderTargetIdentifier target,
            int kernel8,
            int kernel1)
        {
            RectInt main, topRow, rightCol, topRight;
            unsafe
            {
                RectInt* dispatch1Rects = stackalloc RectInt[3];
                int dispatch1RectCount = 0;
                RectInt dispatch8Rect = RectInt.zero;

                if (TileLayoutUtils.TryLayoutByTiles(
                        rect,
                        8,
                        out main,
                        out topRow,
                        out rightCol,
                        out topRight))
                {
                    if (topRow.width > 0 && topRow.height > 0)
                    {
                        dispatch1Rects[dispatch1RectCount] = topRow;
                        ++dispatch1RectCount;
                    }
                    if (rightCol.width > 0 && rightCol.height > 0)
                    {
                        dispatch1Rects[dispatch1RectCount] = rightCol;
                        ++dispatch1RectCount;
                    }
                    if (topRight.width > 0 && topRight.height > 0)
                    {
                        dispatch1Rects[dispatch1RectCount] = topRight;
                        ++dispatch1RectCount;
                    }
                    dispatch8Rect = main;
                }
                else if (rect.width > 0 && rect.height > 0)
                {
                    dispatch1Rects[dispatch1RectCount] = rect;
                    ++dispatch1RectCount;
                }

                cmd.SetComputeTextureParam(m_Shader, kernel8, _source, source);
                cmd.SetComputeTextureParam(m_Shader, kernel1, _source, source);
                cmd.SetComputeTextureParam(m_Shader, kernel8, _target, target);
                cmd.SetComputeTextureParam(m_Shader, kernel1, _target, target);

                if (dispatch8Rect.width > 0 && dispatch8Rect.height > 0)
                {
                    var r = dispatch8Rect;
                    // Caution: passing parameters to SetComputeIntParams() via params generate 48B several times at each frame here !
                    cmd.SetComputeIntParams(m_Shader, _RectOffset, (int)r.x, (int)r.y);
                    cmd.DispatchCompute(m_Shader, kernel8, (int)Mathf.Max(r.width / 8, 1), (int)Mathf.Max(r.height / 8, 1), 1);
                }

                for (int i = 0, c = dispatch1RectCount; i < c; ++i)
                {
                    var r = dispatch1Rects[i];
                    // Caution: passing parameters to SetComputeIntParams() via params generate 48B several times at each frame here !
                    cmd.SetComputeIntParams(m_Shader, _RectOffset, (int)r.x, (int)r.y);
                    cmd.DispatchCompute(m_Shader, kernel1, (int)Mathf.Max(r.width, 1), (int)Mathf.Max(r.height, 1), 1);
                }
            }
        }

        public void SampleCopyChannel_xyzw2x(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, RectInt rect)
        {
            SampleCopyChannel(cmd, rect, _Source4, source, _Result1, target, k_SampleKernel_xyzw2x_8, k_SampleKernel_xyzw2x_1);
        }
    }
}
