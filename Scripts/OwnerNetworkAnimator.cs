using Unity.Netcode.Components;

public class OwnerNetworkAnimator : NetworkAnimator {
    protected override bool OnIsServerAuthoritative() {
        return false;
    }
    //�������ͨ������ȷ�����綯���Ƿ��ɷ��������п��ơ�ͨ��������ֵ��Ϊ false����ʾ���綯�������ɷ��������п��ƣ������ڱ��ؽ��д�����ɿͻ��˽��п��ơ�

//    ��ˣ���δ��붨����һ���Զ�������綯���� OwnerNetworkAnimator��
//    ����ȷָ�������綯�����ܷ�������Ȩ���ƣ�
//    ������ζ�Ÿ����綯���ڱ��ػ�ͻ��˿��������У�������ȫ�ɷ������˿��ơ�
}