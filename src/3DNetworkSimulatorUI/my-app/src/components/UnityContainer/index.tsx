import React, { useCallback, useEffect, useState } from "react";
import { Unity, useUnityContext, } from "react-unity-webgl";
import gnsSettings from '../../configs/server.json';

// Create Unity context
const { unityProvider, sendMessage, addEventListener, removeEventListener } = useUnityContext({
    loaderUrl: "/build/Build/build.loader.js",
    dataUrl: "/build/Build/webgl.data",
    frameworkUrl: "/build/Build/build.framework.js",
    codeUrl: "/build/Build/build.wasm",
});

const [arg, requestGnsCofig] = useState();

const handleRequestGnsCofig = useCallback(() => {
    console.log("Requested configs, sending " + gnsSettings)
    sendMessage(
        "GlobalGnsParameters",
        "SetGnsConfig",
        (gnsSettings.addr, gnsSettings.port, gnsSettings.user)
    );
}, []);

useEffect(() => {
    addEventListener("RequestGnsCofig", requestGnsCofig);
    return () => {
        removeEventListener("RequestGnsCofig", requestGnsCofig);
    };
}, [addEventListener, removeEventListener, handleRequestGnsCofig]);

const UnityContainer: React.FC = () => {
    // Return page
    return < Unity unityProvider={unityProvider} />;
}

export default UnityContainer