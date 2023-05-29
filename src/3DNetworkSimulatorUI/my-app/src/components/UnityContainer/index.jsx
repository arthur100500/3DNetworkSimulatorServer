import React, { useCallback, useEffect, useState } from "react";
import { Unity, useUnityContext, } from "react-unity-webgl";
import gnsSettings from '../../configs/server.json';

const UnityContainer = () => {
    // Create Unity context
    const { unityProvider } = useUnityContext({
        loaderUrl: "/build/Build/build.loader.js",
        dataUrl: "/build/Build/webgl.data",
        frameworkUrl: "/build/Build/build.framework.js",
        codeUrl: "/build/Build/build.wasm",
    });

    const originalfetch = fetch;

    useEffect(() => {
        function overrideFetchAddCredentials() {
            fetch = function (url, data) {
                console.log("url received: " + url);
                if (url.indexOf('https://www.mavon.art/') === 0 || url.indexOf('http://localhost:10203') === 0 || url.indexOf('http://127.0.0.1:10203') === 0) {
                    data = { ...data, ...{ credentials: 'same-origin' } };
                    console.log("withCredentials set to true " + JSON.stringify(data) + " url: " + url);
                } else {
                    console.log("withCredentials NOT SET for URL: " + url);
                }
                return originalfetch(url, data);
            };
            fetch.overridden = true;
        }

        if (fetch.overridden !== true) {
            console.log("Override fetch function");
            overrideFetchAddCredentials();
        } else {
            console.log("fetch function already overridden");
        }
    });
    return < Unity unityProvider={unityProvider} />;
}

export default UnityContainer