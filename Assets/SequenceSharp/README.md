# Sequence for Unity!

## Using the unitypackage

1. Import Vuplex.
2. Import SequenceSharp.unitypackage.
3. Add an Assembly Reference to SequenceSharp's Assembly Definition, and/or SequenceIndexer's Assembly Definition.
   This is required because SequenceSharp must inform Vuplex _not_ to build for WebGL.
   If you're already using Vuplex WebGL for something different, you can delete the Assembly Reference files in Vuplex, in SequenceSharp, and in SequenceIndexer.

## Creating the UnityPackage

Choose everything in the SequenceSharp and SequenceIndexer folders, the StreamingAssets/sequence folder, and the file Vuplex/VuplexAssemblyReference
