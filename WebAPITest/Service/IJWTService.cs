/// <summary>
/// 认证处理接口
/// </summary>
public interface IJWTService
{
    /// <summary>
    /// 创建Token
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string CreateToken(string name, int userId, string email, string nickname);
}