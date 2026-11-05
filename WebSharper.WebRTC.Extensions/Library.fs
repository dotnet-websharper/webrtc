// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
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

open WebSharper
open WebSharper.JavaScript

/// Modern, Promise-based WebRTC helpers that complement the core
/// WebSharper.JavaScript media types. Core already models MediaDevices,
/// MediaStream and HTMLVideoElement (registered in the
/// "WebSharper.JavaScript" namespace), but it does not expose getUserMedia
/// or srcObject, so those are added here.
[<JavaScript; AutoOpen>]
module Extensions =

    /// Promise-based getUserMedia: the current standard API, replacing the
    /// removed navigator.getUserMedia callback API. Resolves with the captured
    /// MediaStream, or rejects with a DOMException.
    [<Inline "$devices.getUserMedia($constraints)">]
    let GetUserMedia (devices: MediaDevices) (constraints: MediaStreamConstraints) : Promise<MediaStream> =
        X<_>

    /// Attaches a MediaStream to a <video> element via srcObject, replacing the
    /// removed URL.createObjectURL(stream) + src attribute approach.
    [<Inline "$video.srcObject = $stream">]
    let SetSrcObject (video: HTMLVideoElement) (stream: MediaStream) : unit =
        X<unit>

    /// Detaches the current MediaStream from a <video> element.
    [<Inline "$video.srcObject = null">]
    let ClearSrcObject (video: HTMLVideoElement) : unit =
        X<unit>
