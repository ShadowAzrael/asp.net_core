using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPITest.Models;

namespace WebAPITest.Service
{
    public class JWTService : IJWTService
    {
        private readonly JWTConfig _jwtConfig;

        /// <summary>
        /// 构造注入 注入jwt配置信息
        /// </summary>
        /// <param name="jwtConfig"></param>
        public JWTService(IOptions<JWTConfig> jwtConfig)
        {
            this._jwtConfig = jwtConfig.Value;
        }
        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string CreateToken(string name, int userId, string email, string nickname)
        {
            //把有需要的信息写到Token
            var claims = new[] {
                //用户的Id
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                //用户名
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email,email),
                //自定义
                new Claim("QQ","673025029"),
                new Claim("NickName",nickname),
                new Claim("Levle","1")

            };

            //创建密钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            //密钥加密
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //token配置
            var jwtToken = new JwtSecurityToken(_jwtConfig.Issuer,
                _jwtConfig.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtConfig.AccessExpiration),
                signingCredentials: credentials);

            //获取token
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }
    }
}
