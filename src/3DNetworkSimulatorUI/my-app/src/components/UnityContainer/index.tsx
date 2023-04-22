import React, { useCallback, useEffect, useState } from "react";
import { Unity, useUnityContext, } from "react-unity-webgl";
import gnsSettings from '../../configs/server.json';

const UnityContainer: React.FC = () => {
    // Create Unity context
    const { unityProvider } = useUnityContext({
        loaderUrl: "/build/Build/build.loader.js",
        dataUrl: "/build/Build/webgl.data",
        frameworkUrl: "/build/Build/build.framework.js",
        codeUrl: "/build/Build/build.wasm",
    });

    return < Unity unityProvider={unityProvider} />;
}

export default UnityContainer