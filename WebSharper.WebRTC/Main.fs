namespace WebRTC

open WebSharper.InterfaceGenerator

module Definition =
    open WebSharper

    let O = T<unit>
    let Event = T<WebSharper.JavaScript.Dom.Event>

    let MediaStreamTrack = Type.New ()

    let MediaStreamTrackState = 
        Pattern.EnumStrings "MediaStreamTrackState" [
            "new"
            "live"
            "ended"
        ]

    let SourceTypeEnum =
        Pattern.EnumStrings "SourceTypeEnum" [
            "nonce"
            "camera"
            "microphone"
        ]    

    module Constraints =
        let DoubleRange =
            Pattern.Config "ConstrainDoubleRange" {
                Required = [ "max", T<double>; "min", T<double> ]
                Optional = []
            }
        let LongRange =
            Pattern.Config "ConstrainLongRange" {
                Required = [ "max", T<int>; "min", T<int> ]
                Optional = []
            }

        let ConstrainString = T<string> + Type.ArrayOf T<string>
        let ConstrainDouble = T<double> + DoubleRange
        let ConstrainLong = T<int> + LongRange
        let FacingMode = 
            Pattern.EnumStrings "VideoFacingMode" [ "left"; "right"; "user"; "environment" ]

        let ConstrainFacingMode = FacingMode + Type.ArrayOf FacingMode

    let MediaTrackConstraintSet = 
        Pattern.Config "MediaTrackContraintSet" {
            Required = []
            Optional = [
                            "width", Constraints.ConstrainLong
                            "height", Constraints.ConstrainLong
                            "aspectRatio", Constraints.ConstrainDouble
                            "frameRate", Constraints.ConstrainDouble
                            "facingMode", Constraints.ConstrainFacingMode
                            "volume", Constraints.ConstrainDouble
                            "sampleRate", Constraints.ConstrainLong
                            "sampleSize", Constraints.ConstrainLong
                            "echoCancelation", T<bool>
                            "sourceId", Constraints.ConstrainString
                       ]
        }

    let MediaTrackContraints =
        Pattern.Config "MediaTrackConstraints" {
            Required = []
            Optional = ["advanced", Type.ArrayOf MediaTrackConstraintSet; "require", T<string> ]
        }
        |=> Inherits MediaTrackConstraintSet

    let MediaStreamTrackEventInit =
        Pattern.Config "MediaStreamTrackEventInit" {
            Required = [ "track", MediaStreamTrack ]
            Optional = []
        }

    let MediaStreamTrackEvent =
        Class "MediaStreamTrackEvent"
        |+> Static [
            Constructor (T<string>?``type`` * MediaStreamTrackEventInit?eventInitDict)
        ]
        |+> Instance [
            "track" =? MediaStreamTrack
            |> WithComment "The MediaStreamTrack associated with this event."
        ]

    let MediaStreamError = 
        Class "MediaStreamError"
        |+> Instance [
            "name" =? T<string>
            |> WithComment "The name of the error."
            "message" =? T<string>
            |> WithComment "A UA-dependent string offering extra human-readable information about the error."
            "constraintName" =? T<string>
        ]

    let MediaStreamErrorEventInit =
        Pattern.Config "MediaStreamErrorEventInit" {
            Required = [ "error", MediaStreamError.Type ]
            Optional = []
        }       

    let MediaStreamErrorEvent =
        Class "MediaStreamErrorEvent"
        |+> Static [
            Constructor (T<string>?``type`` * MediaStreamErrorEventInit?eventInitDict)
        ]
        |+> Instance [
            "error" =? MediaStreamError
            |> WithComment "The error associated with this event."
        ]

    let MediaDeviceKind =
        Pattern.EnumStrings "MediaDeviceKind" [
            "audioinput"
            "audiooutput"
            "videoinput"
        ]

    let MediaDeviceInfo =
        Pattern.Config "MediaDeviceInfo" {
            Required = []
            Optional =  [
                            "deviceId", T<string>
                            "label", T<string>
                            "groupId", T<string>
                            "kind", MediaDeviceKind.Type
                        ]
        }

    let MediaDevices =
        Class "MediaDevices"
        |=> Inherits T<WebSharper.JavaScript.Dom.EventTarget>
        |+> Static [
            "getSupportedConstraints" => T<string>?kind ^-> T<obj>
        ]
        |+> Instance [
            "ondevicechange" =@ Event ^-> O
            "enumerateDevices" => (Type.ArrayOf MediaDeviceInfo ^-> O)?resultCallback ^-> O
            |> WithComment "Collects information about the user agents available media input and output devices."
        ]

    let MediaStreamConstraints =
        Pattern.Config "MediaStreamConstraints" {
            Required =  [
                            "video", T<bool> + MediaTrackContraints
                            "audio", T<bool> + MediaTrackContraints
                        ]

            Optional =  [
                            "peerIdentity", T<string>
                        ]
        }

    let MediaStream = Type.New ()

    let UserMedia =
        Class "UserMedia"
        |+> Static [
            Constructor O
            |> WithInline "navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia"
            ///Not supported
            //"getMediaDevices" => ((Type.ArrayOf MediaDeviceInfo) ^-> O)?resultCallback ^-> O
            //|> WithInline "navigator.getMediaDevices($resultCallback)$"
        ]
        |+> Instance [
            "getUserMedia" => (MediaStreamConstraints?constraints * (MediaStream ^-> O)?successCallback * (MediaStreamError ^-> O)?errorCallback) ^-> O
            |> WithInline "navigator.getUserMedia($constraints, $successCallback, $errorCallback)"
            |> WithComment "Gets the MediaStream track specified by the constraints."
        ]
        

    let MediaStreamTrackClass = 
        Class "MediaStreamTrack"
        |=> MediaStreamTrack
        |=> Inherits T<WebSharper.JavaScript.Dom.EventTarget>
        |+> Instance [
            "kind" =? T<string>
            |> WithComment "Returns audio or video accoring to the contained medium."
            "id" =? T<string>
            |> WithComment "Globally unique identifier."
            "label" =? T<string>
            "enabled" =@ T<bool>
            "muted" =? T<bool>

            "_readonly" =? T<bool>
            "remote" =? T<bool>
            |> WithComment "Shows whether the object is sourced by RTCPeerConnection."
            "readyState" =? MediaStreamTrackState

            "onmute" =@ Event ^-> O
            "onunmute" =@ Event ^-> O
            "onstarted" =@ Event ^-> O
            "onended" =@ Event ^-> O

            "getNativeSettings" => O ^-> T<obj> 
            |> WithComment "Returns the native settings of all the properties of the object."
            "clone" => O ^-> MediaStreamTrack
            |> WithComment "Clones the given MediaStreamTrack."
            "stop" => O ^-> O
            |> WithComment "Detaches the tracks's source."
            "getCapabilities" => O ^-> T<obj>
            |> WithComment "See ConstrainablePattern Interface for the definition of this method."
            "getConstraints" => O ^-> MediaTrackContraints
            "getSettings" => O ^-> T<obj>
            "applyConstraints" => (MediaTrackContraints?constraints * (O ^-> O)?successCallback * (MediaStreamError ^-> O)?errorCallback) ^-> O
            |> WithComment "Applies a set of constraints to the track."
        ]

    let MediaStreamClass = 
        Class "MediaStream"
        |=> MediaStream
        |=> Inherits T<JavaScript.Dom.EventTarget>
        |+> Static [
            Constructor O
            Constructor MediaStream
            |> WithComment "Composes a new stream out of existing tracks."
            Constructor (Type.ArrayOf MediaStream)
            |> WithComment "Composes a new stream out of existing tracks."
        ]
        |+> Instance [
            "id" =? T<string>
            |> WithComment "Globally unqiue identifier."
            "getAudioTracks" => O ^-> Type.ArrayOf MediaStreamTrack
            |> WithComment "Returns a sequence of MediaStreamTrack objects representing the audio tracks in this stream."
            "getVideoTracks" => O ^-> Type.ArrayOf MediaStreamTrack
            |> WithComment "Returns a sequence of MediaStreamTrack objects representing the video tracks in this stream."
            "getTracks" => O ^-> Type.ArrayOf MediaStreamTrack
            |> WithComment "Returns a sequence of MediaStreamTrack objects representing all the tracks in this stream."
            "getTrackById" => T<string>?trackId ^-> MediaStreamTrack
            |> WithComment "Returns the first MediaStreamTrack whose id is equal to trackId from this MediaStream's tracklist."
            "addTrack" => MediaStreamTrack?track ^-> O
            |> WithComment "Adds the given MediaStreamTrack to this MediaStream."
            "removeTrack" => MediaStreamTrack ^-> O
            |> WithComment "Removes the given MediaStreamTrack from this MediaStream."
            "clone" => O ^-> MediaStream
            |> WithComment "Clones the given MediaStream and all its tracks."

            "active" =? T<bool>
            "onactive" =@ Event ^-> O
            "oninactive" =@ Event ^-> O
            "onaddtrack" =@ MediaStreamTrackEvent ^-> O
            "onremovetrack" =@ MediaStreamTrackEvent ^-> O
        ]

    let Assembly = 
        Assembly [
            Namespace "WebSharper.JavaScript" [
                MediaStreamTrackState
                SourceTypeEnum
                Constraints.DoubleRange
                Constraints.LongRange
                Constraints.FacingMode
                MediaTrackConstraintSet
                MediaTrackContraints
                MediaStreamTrackEventInit
                MediaStreamTrackEvent
                MediaStreamError
                MediaStreamErrorEventInit
                MediaStreamErrorEvent
                MediaDeviceKind
                MediaDeviceInfo
                MediaDevices
                MediaStreamConstraints
                UserMedia
                MediaStreamTrackClass
                MediaStreamClass
            ]
        ] 

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
