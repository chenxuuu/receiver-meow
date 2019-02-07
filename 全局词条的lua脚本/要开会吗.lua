local qq,name = message:match("[7|七]号要开会吗%[CQ:at,qq=(%d+)%](.+)")
if qq then
    print(at(qq).."是你们直接@的？"..(name and "现在你是在叫"..name.."？").."我不想再看见第二次")
    print(at(fromqq).."你自己心里没点数？")
    print("请各位群员以后自己的身份和说话方式@全体成员")
end
