using Unity.Netcode.Components;

public class OwnerNetworkAnimator : NetworkAnimator {
    protected override bool OnIsServerAuthoritative() {
        return false;
    }
    //这个方法通常用于确定网络动画是否由服务器进行控制。通过将返回值设为 false，表示网络动画不是由服务器进行控制，而是在本地进行处理或由客户端进行控制。

//    因此，这段代码定义了一个自定义的网络动画类 OwnerNetworkAnimator，
//    并明确指定了网络动画不受服务器授权控制，
//    可能意味着该网络动画在本地或客户端控制下运行，而非完全由服务器端控制。
}