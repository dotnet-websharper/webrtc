// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2026 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}
namespace WebRTC

open WebSharper.InterfaceGenerator

module Definition =
    open WebSharper

    let O = T<unit>

    // Core (WebSharper.JavaScript) already models MediaDevices, MediaStream,
    // MediaStreamTrack, MediaDeviceInfo/MediaDeviceKind, the media constraint
    // types and HTMLVideoElement, and registers them in the
    // "WebSharper.JavaScript" namespace. The binding therefore only adds the
    // pieces core is missing: MediaStreamConstraints, the MediaStreamError
    // family and the legacy UserMedia type.
    //
    // The modern, Promise-based getUserMedia and HTMLVideoElement.srcObject
    // live in the companion WebSharper.WebRTC.Extensions assembly, because
    // their inline JavaScript implementations are regular F# code.

    let MediaStreamConstraints =
        Pattern.Config "MediaStreamConstraints" {
            // Both are optional per the spec: you may request audio only,
            // video only, or neither (e.g. to just prompt for permission).
            Required = []
            Optional = [
                "video", T<bool> + T<WebSharper.JavaScript.MediaTrackConstraints>
                "audio", T<bool> + T<WebSharper.JavaScript.MediaTrackConstraints>
                "peerIdentity", T<string>
            ]
        }

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

    let UserMedia =
        Class "UserMedia"
        |+> Static [
            Constructor O
            |> WithInline "navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia"
        ]
        |+> Instance [
            "getUserMedia" => (MediaStreamConstraints?constraints * (T<WebSharper.JavaScript.MediaStream> ^-> O)?successCallback * (MediaStreamError ^-> O)?errorCallback) ^-> O
            |> WithInline "navigator.getUserMedia($constraints, $successCallback, $errorCallback)"
            |> WithComment "Gets the MediaStream specified by the constraints."
        ]
        // navigator.getUserMedia was removed from browsers years ago; keep the
        // type for source compatibility but steer callers to the modern API.
        |> ObsoleteWithMessage "navigator.getUserMedia was removed from modern browsers. Use the Promise-based getUserMedia on MediaDevices (see WebSharper.WebRTC.Extensions) instead."

    let Assembly =
        Assembly [
            Namespace "WebSharper.JavaScript" [
                MediaStreamConstraints
                MediaStreamError
                MediaStreamErrorEventInit
                MediaStreamErrorEvent
                UserMedia
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
