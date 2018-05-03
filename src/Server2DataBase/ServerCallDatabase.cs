using NCLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server2DataBase
{
    public class ServerCallDatabase : IServerCallDatabase
    {
        #region 静态字段
        private static string checkExit = "select count(*) from 用户信息 where 学号='{0}'";
        private static string checkExit2 = "select count(*) from 用户信息 where 学号='{0}' and 密码='{1}'";
        private static string addUser = "insert into 用户信息 values('{0}','{1}','{2}',0)";
        private static string isPrerogative = "select count(*) from 用户信息 where 学号='{0}' and 权限='true'";
        #endregion

        private static SqlConnection sqlConnection;
        private static bool init = false;
        private DataSet _tmpDataSet = new DataSet();
        /// <summary>
        /// 执行语句结果(只读)
        /// </summary>
        public DataSet TmpDataSet
        {
            get
            {
                return _tmpDataSet;
            }
        }

        /// <summary>
        /// 验证用户信息(或存在)
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <param name="password">密码</param>
        /// <returns>结果</returns>
        private bool IsExist(UserInfo info, string password = null)
        {
            if (password == null)
            {
                if (ExecuteStructuredQueryLanguage(String.Format(checkExit, info.UserId), "IsExist").BaseResult == baseResult.Faild)
                    return false;
                if (((this._tmpDataSet.Tables["IsExist"].Rows)[0])[0].ToString() == "0")
                    return false;
            }
            else
            {
                if (ExecuteStructuredQueryLanguage(String.Format(checkExit2, info.UserId, password), "IsExist").BaseResult == baseResult.Faild)
                    return false;
                if (((this._tmpDataSet.Tables["IsExist"].Rows)[0])[0].ToString() == "0")
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <param name="userId">用户ID</param>
        /// <param name="password">密码</param>
        /// <returns>处理结果</returns>
        public IResult AddUser(UserInfo info, string password)
        {
            if (IsExist(info))
                return new Result(baseResult.Faild, "已存在用户");
            else
            {
                IResult tmpResult = ExecuteStructuredQueryLanguage(String.Format(addUser, info.UserId, info.NickName, password), "AddUser");
                if (tmpResult.BaseResult == baseResult.Faild)
                    return tmpResult;
            }
            return new Result(baseResult.Successful, "成功");
        }

        /// <summary>
        /// 验证用户信息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="password">密码</param>
        /// <returns>结果</returns>
        public IResult CheckUserInfo(UserInfo info, string password)
        {
            if (IsExist(info, password))
                return new Result(baseResult.Successful, "验证成功");
            return new Result(baseResult.Faild, "错误的账号或密码");
        }

        /// <summary>
        /// 是否特权用户
        /// </summary>
        /// <param name="info">用户信息</param>
        /// <returns>结果</returns>
        public bool IsPrerogative(UserInfo info)
        {
            if (ExecuteStructuredQueryLanguage(String.Format(isPrerogative, info.UserId), "IsPrerogative").BaseResult == baseResult.Faild)
                return false;
            if (((this._tmpDataSet.Tables["IsPrerogative"].Rows)[0])[0].ToString() == "0")
                return false;
            return true;
        }

        /// <summary>
        /// 断开数据库连接
        /// </summary>
        public void CloseDatabase()
        {
            if (init)
                sqlConnection.Close();
            init = false;
        }

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="user">数据库账号</param>
        /// <param name="password">密码</param>
        /// <param name="url">数据库地址</param>
        /// <param name="database">数据库名称</param>
        /// <returns>链接结果</returns>
        public IResult ConnectDatabase(string user, string password, string url, string database)
        {
            try
            {
                string connString = String.Format("server={0};database ={1};uid ={2};pwd={3}", url, database, user, password);
                sqlConnection = new SqlConnection(connString);
                sqlConnection.Open();
                init = true;
            }
            catch (Exception e)
            {
                init = false;
                return new Result(baseResult.Faild, e.Message);
            }
            return new Result(baseResult.Successful);
        }

        public void DeletUser(string userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 执行SQL结构化查询语句
        /// </summary>
        /// <param name="sql">结构化查询语句</param>
        /// <param name="tableTitle">结果标题</param>
        /// <returns>执行结果</returns>
        public IResult ExecuteStructuredQueryLanguage(string sql, string tableTitle)
        {
            if (!init)
                return new Result(baseResult.Faild, "未连接数据库");
            //定义一个数据库操作指令
            SqlCommand MyCommand = new SqlCommand(sql, sqlConnection);
            //定义一个数据适配器
            SqlDataAdapter SelectAdapter = new SqlDataAdapter();
            //定义数据适配器的操作指令
            SelectAdapter.SelectCommand = MyCommand;
            try
            {
                SelectAdapter.SelectCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                return new Result(baseResult.Faild, e.Message);
            }
            //填充数据集
            if (this._tmpDataSet.Tables.Contains(tableTitle))
                this._tmpDataSet.Tables[tableTitle].Clear();
            SelectAdapter.Fill(this._tmpDataSet, tableTitle);
            return new Result(baseResult.Successful);
        }

    }
}
