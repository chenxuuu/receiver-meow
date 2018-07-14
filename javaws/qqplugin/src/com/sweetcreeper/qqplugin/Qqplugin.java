package com.sweetcreeper.qqplugin;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.PrintWriter;
import java.net.Socket;
import java.net.UnknownHostException;

import org.bukkit.Bukkit;
import org.bukkit.OfflinePlayer;
import org.bukkit.entity.Player;
import org.bukkit.event.EventHandler;
import org.bukkit.event.Listener;
import org.bukkit.event.entity.PlayerDeathEvent;
import org.bukkit.event.player.AsyncPlayerChatEvent;
import org.bukkit.event.player.PlayerJoinEvent;
import org.bukkit.event.player.PlayerQuitEvent;
import org.bukkit.plugin.RegisteredServiceProvider;
import org.bukkit.plugin.java.JavaPlugin;
import org.bukkit.scheduler.BukkitRunnable;

import net.milkbowl.vault.economy.Economy;


public class Qqplugin extends JavaPlugin implements Listener
{
	public static String player="none233";
	public static String msg="none233";
    public static boolean isEco = false;
    public static Economy economy;	
    public static boolean isRev = false;
    public static String[] sourceStrArray;
	
	@Override//重写父类的方法
	public void onEnable()
	{
		getLogger().info("QQGroupMessagePlugin is started successfully!");
		
		player="ok";
		msg="<提示>服务器已重启完成。";
		//注册监听
		Bukkit.getPluginManager().registerEvents(this,this);
		
		new BukkitRunnable(){     
		    int s = 0;//设置定10秒后执行某段代码
		    @Override    
		    public void run(){
		    	if(s>=60*20)
		    	{
		    		Bukkit.getServer().dispatchCommand(Bukkit.getServer().getConsoleSender(), "tm abc 发钱啦！");
		    		Bukkit.getServer().dispatchCommand(Bukkit.getServer().getConsoleSender(), "eco give * 1");
		    		s=0;
		    	}
		    	else
		    	{
		    		s++;
		    	}
		    	
		    	if(isRev)
		    	{
		    		Bukkit.broadcastMessage("这是测试"+s+"计数,遇到阻碍");
		    		return;
		    	}
		    	else
		    	{
		    		//Bukkit.broadcastMessage("这是测试"+s+"计数,执行正常");
		    	}
		        //s--;//迭代递减,我看官方的教程是没这个的,我没试过,你也可以删除试试
		        //if(s==0){
		            //这个写10秒后执行的代码(假如定义的定时器每次是1秒)
		        //    cancel();//cancel用来取消定时器
		        //}else{
		            //这里可以写每次触发定时器执行的代码
		    	isRev = true;
		            try 
		            {
		                //1.创建客户端Socket，指定服务器地址和端口
		                Socket socket=new Socket("localhost", 2333);
		                //2.获取输出流，向服务器端发送信息
		                OutputStream os=socket.getOutputStream();//字节输出流
		                PrintWriter pw=new PrintWriter(os);//将输出流包装为打印流
		                if(player!="none233")
		                {
		                	pw.write(msg);
		                	player="none233";
		                }
		                else
		                {
		                	pw.write("getmsg");
		                }
		                pw.flush();
		                socket.shutdownOutput();//关闭输出流
		                //3.获取输入流，并读取服务器端的响应信息
		                InputStream is=socket.getInputStream();
		                BufferedReader br=new BufferedReader(new InputStreamReader(is));
		                String info=null;
		                info=br.readLine();
		                //4.关闭资源
		                br.close();
		                is.close();
		                pw.close();
		                os.close();
		                socket.close();
		                
		                sourceStrArray=info.split("\\|\\|\\|\\|\\|");
		                for(int i=0;i<sourceStrArray.length;i++)
		                {
		                	if(sourceStrArray[i].indexOf("<")!=-1)
		                	{
		                		//Bukkit.broadcastMessage(sourceStrArray[i]);
		                		Bukkit.getServer().dispatchCommand(Bukkit.getServer().getConsoleSender(), "say "+sourceStrArray[i]);
		                	}
		                	else if(sourceStrArray[i].indexOf("command>")!=-1)
		                	{
		                		Bukkit.getServer().dispatchCommand(Bukkit.getServer().getConsoleSender(), sourceStrArray[i].replace("command>", ""));
		                		if(player!="none233")
		                		{
		                			msg+="]][[<提示>"+sourceStrArray[i].replace("command>", "")+"已执行";
		                		}
		                		else
		                		{
		                			player="ok";
		                			msg="<提示>"+sourceStrArray[i].replace("command>", "")+"已执行";
		                		}
		                	}
		                	else if(sourceStrArray[i].indexOf("sum>")!=-1)
		                	{
		                		String result="";
		                		for(Player p : Bukkit.getOnlinePlayers())
		                		{
		                			result+=p.getName()+",";
		                		}
		                		if(player!="none233")
		                		{
		                			msg+="]][[<提示>服务器当前在线人数为"+Bukkit.getOnlinePlayers().size()+"人，玩家列表："+result;
		                		}
		                		else
		                		{
		                			player="ok";
		                			msg="<提示>服务器当前在线人数为"+Bukkit.getOnlinePlayers().size()+"人，玩家列表："+result;
		                		}
		                	}
		                	else if(sourceStrArray[i].indexOf("eco>")!=-1)
		                	{
		                		String result="",playername=sourceStrArray[i].replace("eco>", "");
		                        isEco = setupEconomy();
		                        if(isEco)
		                        {
		                        	@SuppressWarnings("deprecation")
									OfflinePlayer p = Bukkit.getOfflinePlayer(playername);
		                            //获取玩家金钱
		                        	if(p!=null)
		                        		result = "玩家"+ playername +"服务器余额为：" + economy.getBalance(p);
		                        }
		                        else
		                        {
		                            //valut没装或者ess经济没装
		                        	result="玩家"+ playername +"服务器查询余额失败。";
		                        }
		                		if(player!="none233")
		                		{
		                			msg+="]][[<提示>"+result;
		                		}
		                		else
		                		{
		                			player="ok";
		                			msg="<提示>"+result;
		                		}
		                	}
		                	else if(sourceStrArray[i].indexOf("ecodel100>")!=-1)
		                	{
		                		String result="",playername=sourceStrArray[i].replace("ecodel100>", "");
		                        isEco = setupEconomy();
		                        if(isEco)
		                        {
		                        	@SuppressWarnings("deprecation")
									OfflinePlayer p = Bukkit.getOfflinePlayer(playername);
		                            if(economy.has(p,100) && p!=null)//判断玩家是否100元
		                            {
		                            	economy.withdrawPlayer(p,100);//扣除100元
		                            	result = "<eco100>" + playername;
		                            }
		                            else
		                            {
		                            	result = "<提示>"+playername + "你服务器里的钱不够100。";
		                            }
		                        }
		                        else
		                        {
		                            //valut没装或者ess经济没装
		                        	result="玩家"+ playername +"服务器查询余额失败。";
		                        }
		                		if(player!="none233")
		                		{
		                			msg+="]][["+result;
		                		}
		                		else
		                		{
		                			player="ok";
		                			msg=result;
		                		}
		                	}
		                	else if(sourceStrArray[i].indexOf("ecodel500>")!=-1)
		                	{
		                		String result="",playername=sourceStrArray[i].replace("ecodel500>", "");
		                        isEco = setupEconomy();
		                        if(isEco)
		                        {
		                        	@SuppressWarnings("deprecation")
									OfflinePlayer p = Bukkit.getOfflinePlayer(playername);
		                            if(economy.has(p,500) && p!=null)//判断玩家是否500元
		                            {
		                            	economy.withdrawPlayer(p,500);//扣除500元
		                            	result = "<eco500>" + playername;
		                            }
		                            else
		                            {
		                            	result = "<提示>"+playername + "你服务器里的钱不够500。";
		                            }
		                        }
		                        else
		                        {
		                            //valut没装或者ess经济没装
		                        	result="玩家"+ playername +"服务器查询余额失败。";
		                        }
		                		if(player!="none233")
		                		{
		                			msg+="]][["+result;
		                		}
		                		else
		                		{
		                			player="ok";
		                			msg=result;
		                		}
		                	}
		                	else if(sourceStrArray[i].indexOf("ecodel1000>")!=-1)
		                	{
		                		String result="",playername=sourceStrArray[i].replace("ecodel1000>", "");
		                        isEco = setupEconomy();
		                        if(isEco)
		                        {
		                        	@SuppressWarnings("deprecation")
									OfflinePlayer p = Bukkit.getOfflinePlayer(playername);
		                            if(economy.has(p,1000) && p!=null)//判断玩家是否1000元
		                            {
		                            	economy.withdrawPlayer(p,1000);//扣除1000元
		                            	result = "<eco1000>" + playername;
		                            }
		                            else
		                            {
		                            	result = "<提示>"+playername + "你服务器里的钱不够1000。";
		                            }
		                        }
		                        else
		                        {
		                            //valut没装或者ess经济没装
		                        	result="玩家"+ playername +"服务器查询余额失败。";
		                        }
		                		if(player!="none233")
		                		{
		                			msg+="]][["+result;
		                		}
		                		else
		                		{
		                			player="ok";
		                			msg=result;
		                		}
		                	}
		                	else if(sourceStrArray[i].indexOf("ecodel5000>")!=-1)
		                	{
		                		String result="",playername=sourceStrArray[i].replace("ecodel5000>", "");
		                        isEco = setupEconomy();
		                        if(isEco)
		                        {
		                        	@SuppressWarnings("deprecation")
									OfflinePlayer p = Bukkit.getOfflinePlayer(playername);
		                            if(economy.has(p,5000) && p!=null)//判断玩家是否5000元
		                            {
		                            	economy.withdrawPlayer(p,5000);//扣除5000元
		                            	result = "<eco5000>" + playername;
		                            }
		                            else
		                            {
		                            	result = "<提示>"+playername + "你服务器里的钱不够5000。";
		                            }
		                        }
		                        else
		                        {
		                            //valut没装或者ess经济没装
		                        	result="玩家"+ playername +"服务器查询余额失败。";
		                        }
		                		if(player!="none233")
		                		{
		                			msg+="]][["+result;
		                		}
		                		else
		                		{
		                			player="ok";
		                			msg=result;
		                		}
		                	}
		                	else if(sourceStrArray[i].indexOf("ecodel10000>")!=-1)
		                	{
		                		String result="",playername=sourceStrArray[i].replace("ecodel10000>", "");
		                        isEco = setupEconomy();
		                        if(isEco)
		                        {
		                        	@SuppressWarnings("deprecation")
									OfflinePlayer p = Bukkit.getOfflinePlayer(playername);
		                            if(economy.has(p,10000) && p!=null)//判断玩家是否5000元
		                            {
		                            	economy.withdrawPlayer(p,10000);//扣除5000元
		                            	result = "<eco10000>" + playername;
		                            }
		                            else
		                            {
		                            	result = "<提示>"+playername + "你服务器里的钱不够10000。";
		                            }
		                        }
		                        else
		                        {
		                            //valut没装或者ess经济没装
		                        	result="玩家"+ playername +"服务器查询余额失败。";
		                        }
		                		if(player!="none233")
		                		{
		                			msg+="]][["+result;
		                		}
		                		else
		                		{
		                			player="ok";
		                			msg=result;
		                		}
		                	}
		                }
		                //Bukkit.broadcastMessage("debug:"+info);
		            } catch (UnknownHostException e) {
		                e.printStackTrace();
		            } catch (IOException e) {
		                e.printStackTrace();
		            }
		        //}
		        isRev = false;
		    } 
		}.runTaskTimer(this, 0L, 1L);//参数是,主类、延迟、多少秒运行一次,比如5秒那就是5*20L
	}
	@Override
	public void onDisable()
	{
		if(player!="none233")
		{
			msg+="<提示>服务器已关闭。";
		}
		else
		{
			player="ok";
			msg="<提示>服务器已关闭。";
		}
		getLogger().info("QQGroupMessagePlugin is stoped successfully!");
	}
	
	@EventHandler
	public void onPlayerSay(AsyncPlayerChatEvent event)
	{
		if(player!="none233")
		{
			msg+="]][[<"+event.getPlayer().getName()+">"+event.getMessage();
			//player="ok";
		}
		else
		{
			player="ok";
			msg="<"+event.getPlayer().getName()+">"+event.getMessage();
		}
		
		//Bukkit.broadcastMessage("player:"+event.getPlayer().getName()+",msg:"+event.getMessage());
	}
	
	@EventHandler
	public void onPlayerJoin(PlayerJoinEvent event)
	{
		if(player!="none233")
		{
			msg+="]][[<消息>"+event.getPlayer().getName()+"上线了";
		}
		else
		{
			player="ok";
			msg="<消息>"+event.getPlayer().getName()+"上线了";
		}
		
		if(player!="none233")
		{
			msg+="]][[<qd>"+event.getPlayer().getName();
		}
		else
		{
			player="ok";
			msg="<qd>"+event.getPlayer().getName();
		}
	}
	
	@EventHandler
	public void onPlayerQuit(PlayerQuitEvent event)
	{
		if(player!="none233")
		{
			msg+="]][[<消息>"+event.getPlayer().getName()+"掉线了";
		}
		else
		{
			player="ok";
			msg="<消息>"+event.getPlayer().getName()+"掉线了";
		}
	}
	
	@EventHandler
	public void onPlayerDeath(PlayerDeathEvent e)
	{
	    //Player d = e.getEntity();
	    //Player k = e.getEntity().getKiller();
	    //String i =d.getDisplayName() + "被" + k.getDisplayName() + "杀死了。";
		String i = e.getDeathMessage();
		if(player!="none233")
		{
			msg+="]][[<消息>" + i;
		}
		else
		{
			player="ok";
			msg="<消息>" + i;
		}
	}
	
	
    private boolean setupEconomy() {
		if(Bukkit.getPluginManager().isPluginEnabled("Vault")){
			RegisteredServiceProvider<Economy> economyProvider = getServer().getServicesManager()
					.getRegistration(net.milkbowl.vault.economy.Economy.class);
			if (economyProvider != null) {
				economy = economyProvider.getProvider();
			}
			return (economy != null);
		}else{
			return false;
		}
	}
	/*
	public void socket(String player, String msg)
	{
        try 
        {
            //1.创建客户端Socket，指定服务器地址和端口
            Socket socket=new Socket("localhost", 2333);
            //2.获取输出流，向服务器端发送信息
            OutputStream os=socket.getOutputStream();//字节输出流
            PrintWriter pw=new PrintWriter(os);//将输出流包装为打印流
            pw.write("<"+player+">"+msg);
            pw.flush();
            socket.shutdownOutput();//关闭输出流
            //3.获取输入流，并读取服务器端的响应信息
            //InputStream is=socket.getInputStream();
            //BufferedReader br=new BufferedReader(new InputStreamReader(is));
            //String info=null;
            //while((info=br.readLine())!="msg ok!"){
            	//Bukkit.broadcastMessage(br.readLine());
            //}
            //4.关闭资源
            //br.close();
            //is.close();
            pw.close();
            os.close();
            socket.close();
        } catch (UnknownHostException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
	}*/
}

