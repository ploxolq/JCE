﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JCE.Utils.Extensions;
using JCE.Utils.Logs.Abstractions;
using JCE.Utils.Logs.Extensions;

namespace JCE.Utils.Logs.Core
{
    /// <summary>
    /// 日志操作抽象基类
    /// </summary>
    /// <typeparam name="TContent">日志内容类型</typeparam>
    public abstract class LogBase<TContent>:ILog where TContent:class,ILogContent
    {
        #region Property(属性)
        /// <summary>
        /// 日志内容
        /// </summary>
        private TContent _content;

        /// <summary>
        /// 日志内容
        /// </summary>
        private TContent LogContent => _content ?? (_content = GetContent());

        /// <summary>
        /// 日志提供程序
        /// </summary>
        public ILogProvider Provider { get; }

        /// <summary>
        /// 日志上下文
        /// </summary>
        public ILogContext Context { get; }

        /// <summary>
        /// 调试级别是否启用
        /// </summary>
        public bool IsDebugEnabled => Provider.IsDebugEnabled;

        /// <summary>
        /// 跟踪级别是否启用
        /// </summary>
        public bool IsTraceEnabled => Provider.IsTraceEnabled;
        #endregion

        #region Constructor(构造函数)
        /// <summary>
        /// 初始化一个<see cref="LogBase{TContent}"/>类型的实例
        /// </summary>
        /// <param name="provider">日志提供程序</param>
        /// <param name="context">日志上下文</param>
        protected LogBase(ILogProvider provider, ILogContext context)
        {
            Provider = provider;
            Context = context;
        }
        #endregion

        /// <summary>
        /// 获取日志内容
        /// </summary>
        /// <returns></returns>
        protected abstract TContent GetContent();

        /// <summary>
        /// 设置内容
        /// </summary>
        /// <typeparam name="T">日志内容类型</typeparam>
        /// <param name="action">设置内容操作</param>
        /// <returns></returns>
        public ILog Set<T>(Action<T> action) where T : ILogContent
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            ILogContent content = LogContent;
            action((T) content);
            return this;
        }

        /// <summary>
        /// 跟踪
        /// </summary>
        public void Trace()
        {
            _content = LogContent;
            Execute(ref _content,LogLevel.Trace);
        }
        
        /// <summary>
        /// 跟踪
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">参数值</param>
        public void Trace(string message, params object[] args)
        {
            LogContent.Content(message,args);
            Trace();
        }

        /// <summary>
        /// 调试
        /// </summary>
        public void Debug()
        {
            _content = LogContent;
            Execute(ref _content,LogLevel.Debug);
        }

        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">参数值</param>
        public void Debug(string message, params object[] args)
        {
            LogContent.Content(message, args);
            Debug();
        }

        /// <summary>
        /// 信息
        /// </summary>
        public void Info()
        {
            _content = LogContent;
            Execute(ref _content,LogLevel.Information);
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">参数值</param>
        public void Info(string message, params object[] args)
        {
            LogContent.Content(message, args);
            Info();
        }

        /// <summary>
        /// 警告
        /// </summary>
        public void Warn()
        {
            _content = LogContent;
            Execute(ref _content,LogLevel.Warning);
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">参数值</param>
        public void Warn(string message, params object[] args)
        {
            LogContent.Content(message, args);
            Warn();
        }

        /// <summary>
        /// 错误
        /// </summary>
        public void Error()
        {
            _content = LogContent;
            Execute(ref _content,LogLevel.Error);
        }

        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">参数值</param>
        public void Error(string message, params object[] args)
        {
            LogContent.Content(message,args);
            Error();
        }

        /// <summary>
        /// 致命错误
        /// </summary>
        public void Fatal()
        {
            _content = LogContent;
            Execute(ref _content,LogLevel.Fatal);
        }

        /// <summary>
        /// 致命错误
        /// </summary>
        /// <param name="message">日志消息</param>
        /// <param name="args">参数值</param>
        public void Fatal(string message, params object[] args)
        {
            LogContent.Content(message, args);
            Fatal();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="content">日志内容</param>
        protected virtual void Init(TContent content)
        {
            content.LogName = Provider.LogName;
            content.TraceId = Context.TraceId;
            content.OperationTime = DateTime.Now.ToMillisecondString();
            content.Duration = Context.Stopwatch.Elapsed.Description();
            content.Ip = Context.Ip;
            content.Host = Context.Host;
            content.ThreadId = Thread.CurrentThread.ManagedThreadId.ToString();
            content.Browser = Context.Browser;
            content.Url = Context.Url;
            content.UserId = "";//此处UserID暂为空
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="level">日志级别</param>
        protected virtual void Execute(ref TContent content, LogLevel level)
        {
            try
            {
                Init(content);
                WriteLog(content, level);
            }
            finally
            {
                content = null;
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="level">日志级别</param>
        private void WriteLog(TContent content, LogLevel level)
        {

        }
    }
}