namespace LibsqlClient;

public class Client
{
    public static int Add(int x, int y) => ClientLib.Client.my_add(x, y);
}