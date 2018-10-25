namespace PluginsFSharp

open System.Threading.Tasks
open LEDerZaumzeug
open LEDerZaumzeug.Outputs
open System.Collections.Generic
open System

type FFilter =
    interface IFilter with
        member this.GetInfos(): Task<FilterInfos> = 
            raise (System.NotImplementedException())
        member x.Initialize(mp:MatrixParams): Task =
            Task.CompletedTask;
        member x.Filter(src :RGBPixel[,], frm : UInt64) : Task<RGBPixel[,]> =
            Task.FromResult(src);


type FMixer = 
    interface IMixer with
        member x.Initialize(matrixParameters:MatrixParams): Task =
            Task.CompletedTask
        member x.Mix(sources: IList<RGBPixel[,]> , frame: UInt64 ) : Task<RGBPixel[,]> =
            Task.FromResult(sources.[0]) 


type FGenerator =
    interface IGenerator with
        member this.Dispose(): unit = 
            raise (System.NotImplementedException())
        member x.Initialize(matrixParameters:MatrixParams): Task =
            x.sizeX <- matrixParameters.SizeX;
            x.sizeY <- matrixParameters.SizeY;
            Task.CompletedTask
        member x.GenPattern(frame: UInt64 ) : Task<RGBPixel[,]> =
            Task.FromResult( Array2D.create ((int)x.sizeX) ((int)x.sizeY) RGBPixel.P0); 
    val mutable sizeX: uint32
    val mutable sizeY: uint32


type FOutput =
    inherit OutputBase 
        override x.Initialize(config: LEDerConfig) : Task<bool> =
            Task.FromResult(true)
        override x. Play(pixels:RGBPixel[,]) : Task=
            Task.CompletedTask
        override x.SetSize(rechenDimX: int, rechenDimY: int) : unit =
            x.SizeX <- rechenDimX
            x.SizeY <- rechenDimY

