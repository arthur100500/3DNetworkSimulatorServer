namespace _3DNetworkSimulatorAPI.Views

open Giraffe.ViewEngine

module Views = 
    let baseView content = html [] [
        head [] [ title [] [ str "Network Simulator" ] ]
        body [] [ 
            content
        ]
    ]

    let helloView = 
        h1 [] [str "Hello"]

    let gameView gameLocation =
        iframe [
            attr "mozallowfullscreen" "true";
            attr "allow" "autoplay";
            attr "fullscreen" "";
            _src gameLocation;
            _name "Game";
            attr "scrolling" "no";
            attr "msallowfullscreen" "true";
            attr "allowfullscreen" "true";
            attr "webkitallowfullscreen" "true";
            attr "allowtransparency" "true"
            attr "frameborder" "0"
            _style "border:0px #000000 none;";
            attr "marginheight" "px";
            attr "marginwidth" "320px";
            _height "540px";
            _width "960px";
        ] []