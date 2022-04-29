using System;
using System.Collections.Generic;
using System.Text;

namespace Authgear.Xamarin
{
    /// <summary>
    /// The reason why SessionState is changed.
    ///
    /// These reasons can be thought of as the transition of a SessionState, which is described as follows:
    /// <code>
    ///                                                            LOGOUT / INVALID / Clear
    ///                                                  +-----------------------------------------+
    ///                                                  v                                         |
    /// SessionState.UNKNOWN --- NO_TOKEN ----> SessionState.NO_SESSION ---- AUTHENTICATED -----> SessionState.AUTHENTICATED
    ///      |                                                                                    ^
    ///      +------------------------------------------------------------------------------------+
    ///                                              FOUND_TOKEN
    /// </code>
    /// </summary>
    public enum SessionStateChangeReason
    {
        NoToken,
        FoundToken,
        Authenciated,
        Logout,
        Invalid,
        Clear
    }
}
