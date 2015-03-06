﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using SkinnedModelData;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace SkinnedModelPipeline
{
    [ContentTypeWriter]
    public class ModelExtraWriter : ContentTypeWriter<ModelExtra>
    {
        protected override void Write(ContentWriter output, ModelExtra extra)
        {
            output.WriteObject(extra.Skeleton);
            output.WriteObject(extra.Clips);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ModelExtraReader).AssemblyQualifiedName;
        }
    }
}
