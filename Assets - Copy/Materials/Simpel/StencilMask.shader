Shader "Unlit/StencilMask"
{
    // _StencilID determines what world will be rendered
    Properties
    {
        [IntRange] _StencilID("Stencil ID", Range(0,10)) = 0
    }
        SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            // Write stencil value before any other shader needs to read it
            // Solution : Run stencil mask shader before the Opaque geometry queue
            "Queue" = "Geometry"
        }
        Pass
        {
                // For the color of this pixel, take 0% of the color 
                // output by this shader, 100% of the color already 
                // rendered  at this pixel
                Blend Zero One

                // Prevent this shader from writing to the depth buffer
                ZWrite Off

            // A stencil block : stencil logic inside
            Stencil
            {
                // Reference value
                Ref[_StencilID]

                // ================================================
                // Stencil test : compares the "Ref" value with 
                // stencil value set on a certain pixel
                // Depending on Pass / Fail / ... of this pixel 
                // we change the stencil value  
                // Also have to pass a depth test.
                // ================================================

                // Comp Always : test always pass
                // Comp Never : test always fail
                Comp Always

                // ================================================
                // Instructions of what to do when test pass or fail
                // ================================================

                // (Depth:Pass && Stencil:Pass) = Replace stencil value
                // attached to pixel with the new value defined in this shader
                Pass Replace

                // (Depth:Fail || Stencil:Fail) = Keep the value of the pixel
                Fail Keep
            }
        }

    }
}
