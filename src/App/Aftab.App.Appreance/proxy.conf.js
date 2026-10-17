const { env } = require('process');

// این آدرس حالا مستقیماً به پروژه Monolith شما (ASP.NET Core) اشاره می‌کند
const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(';')[0]
    : 'https://localhost:7001'; // پورت API مونولیت خودت رو اینجا چک کن

const PROXY_CONFIG = [
  {
    context: [
      '/api', // تمام ریکوئست‌های API مستقیماً به مونولیت می‌روند

      // '/hubs' // (اختیاری) اگر در آینده خواستی از SignalR استفاده کنی، این خط رو از کامنت در بیار
    ],
    target,
    secure: false, // نادیده گرفتن خطای سرتیفیکیت در محیط لوکال
    changeOrigin: true,
  },
];

module.exports = PROXY_CONFIG;
