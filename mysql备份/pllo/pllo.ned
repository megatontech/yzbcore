
*
A3                !        )        0 ù
admin2
idint
0: B R Z b hp2@
accountvarchar0: Bè´¦å·Rutf8mb4Zutf8mb4_general_cib 2A
passwordvarcharÿ0: Bå¯ç Rutf8mb4Zutf8mb4_general_cib 2<
namevarchar@0: Båç§°Rutf8mb4Zutf8mb4_general_cib 2;

is_disabletinyint0: Bæ¯å¦ç¦ç¨ 0-å¦ 1-æ¯R Z b 20
create_operator_idint0: B	åå»ºäººR Z b 2,
create_timeint0: Båå»ºæ¶é´R Z b 2,
update_timeint0: Bæ´æ°æ¶é´R Z b 2,
delete_timeint: Bå é¤æ¶é´HR Z b JadminidRplloX`h

auth_group2
idint
0: B R Z b hp29
titlevarchard0: B HRutf8mb4Zutf8mb4_general_cib 2"
statustinyint0:1B HR Z b 2:
rulesvarchar0: B HRutf8mb4Zutf8mb4_general_cib 25
create_operator_idint
0:0B	åå»ºäººHR Z b hJ
auth_groupidRplloX`hÐ
auth_group_access2
uidint
0: B R Z b h2
group_idint
0: B R Z b h:)
uid_group_id "
uid "
group_id :
uid  "
uid :
group_id  "
group_id J
 auth_group_accessRplloX`h
	auth_rule2#
idint
0: Bç¶çº§idR Z b hp2<
namevarcharP0: Bèç¹Rutf8mb4Zutf8mb4_general_cib 26
titlechar0: B HRutf8mb4Zutf8mb4_general_cib 2 
typetinyint0:1B HR Z b 2"
statustinyint0:1B HR Z b 2
pidint
: B HR Z b h2$
sortint
0:0BæåºHR Z b h2>
	conditionvarchar0: B HRutf8mb4Zutf8mb4_general_cib J	auth_ruleidRplloX`hì
	birdhouse2
idint0: B R Z b hp2&
	member_idint0: Bç¨æ·IDR Z b 2<
namevarchar0: Båç§°Rutf8mb4Zutf8mb4_general_cib 2
typetinyint0: Bgç±»å 1-ä¿è²çª 2-ç§å¬çª 3-åºä¹³æ¯çª 4-ç©ºæ å¦å¨ æ¯çª 5-çé¿è²è¥çª 6-éç¦½ 7-æç¦½R Z b 2F
equipment_idvarchar0: Bè®¾å¤IDRutf8mb4Zutf8mb4_general_cib 26
min_temperaturedecimal : Bæå°æ¸©åº¦HR Z b 26
max_temperaturedecimal : Bæå¤§æ¸©åº¦HR Z b 23
min_humiditydecimal : Bæå°æ¹¿åº¦HR Z b 23
max_humiditydecimal : Bæå¤§æ¹¿åº¦HR Z b 2J
warning_ammonia_concentrationdecimal : Bæå°æ°¨æ°æµåº¦HR Z b 2@
warning_negative_pressuredecimal : Bæå°è´åHR Z b 2,
create_timeint0: Båå»ºæ¶é´R Z b 2,
update_timeint0: Bæ´æ°æ¶é´R Z b 2,
delete_timeint: Bå é¤æ¶é´HR Z b J	birdhouseidRplloX`h«
	equipment2
idint
0: B R Z b hp2?
serialvarcharÿ0: Bç¼å·Rutf8mb4Zutf8mb4_general_cib 2=
namevarcharÿ0: Båç§°Rutf8mb4Zutf8mb4_general_cib 2)
birdhouse_idint: Bç¦½èIDHR Z b 2&
	member_idint: Bç¨æ·IDHR Z b 2K
bind_statustinyint0:0B$ç»å®ç¶æ 1-å·²ç»å® 0-æªç»å®HR Z b 2:
statustinyint0:1Bè¿è¡ç¶æ 1-å¼ 0-å³HR Z b 2,
create_timeint0: Båå»ºæ¶é´R Z b 2,
update_timeint0: Bæ´æ°æ¶é´R Z b 2,
delete_timeint: Bå é¤æ¶é´HR Z b J	equipmentidRplloX`h
equipment_status_logs2E
serialvarcharÿ0: Bè®¾å¤ç¼å·Rutf8mb4Zutf8mb4_general_cib 2,
temperaturedecimal 0: Bæ¸©åº¦R Z b 2)
humiditydecimal 0: Bæ¹¿åº¦R Z b 2F
power_statustinyint0: B!çµæºç¶æ 0-å¼ 1-å³ 2-å¼å¸¸R Z b 2,
create_timeint0: Båå»ºæ¶é´R Z b 2,
update_timeint0: Bæ´æ°æ¶é´R Z b J
 equipment_status_logsRplloX`hõ
member2
idint
0: B R Z b hp2@
usernamevarchar0: Bè´¦å·Rutf8mb4Zutf8mb4_general_cib 2A
	nick_namevarchar: Bæµç§°HRutf8mb4Zutf8mb4_general_cib 2?
avatarvarcharÿ: Bå¤´åHRutf8mb4Zutf8mb4_general_cib 2A
mobilevarchar0: B	ææºå·Rutf8mb4Zutf8mb4_general_cib 2O
warning_mobilevarcharÿ0: Bé¢è­¦ææºHRutf8mb4Zutf8mb4_general_cib 2,
create_timeint0: Båå»ºæ¶é´R Z b 2,
update_timeint0: Bæ´æ°æ¶é´R Z b JmemberidRplloX`hÎ
sms_logs2
idint
0: B R Z b hp2A
mobilevarchar0: B	ææºå·Rutf8mb4Zutf8mb4_general_cib 2?
codevarchar0: B	éªè¯ç Rutf8mb4Zutf8mb4_general_cib 2)
end_timeint0: Bè¿ææ¶é´R Z b 2,
create_timeint0: Båå»ºæ¶é´R Z b 2,
update_timeint0: Bæ´æ°æ¶é´R Z b Jsms_logsidRplloX`hÀ
warning_logs2
idint
0: B R Z b hp2&
	member_idint0: Bç¨æ·IDR Z b 2)
birdhouse_idint0: Bç¦½èIDR Z b 2D
causevarcharÿ0: Bæ¥è­¦åå Rutf8mb4Zutf8mb4_general_cib 2,
create_timeint0: Båå»ºæ¶é´R Z b 2,
update_timeint0: Bæ´æ°æ¶é´R Z b Jwarning_logsidRplloX`h"ó
	Diagram 1(0:B
@
admin$´ (»2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:i"g
adminpllo$ (0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h :G
E

auth_groupà· ({2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:o"m

auth_groupplloà (0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h :M
K
auth_group_access$ ¤(K2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:u"s
auth_group_accesspllo$ ñ(0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h :G
E
	auth_rule´ («2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:n"l
	auth_rulepllo (0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h :E
C
	birdhouse$$ (È2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:l"j
	birdhousepllo$ (0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h :F
D
	equipment$ (È2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:m"k
	equipmentpllo (0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h :S
Q
equipment_status_logsà´ ½(2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:z"x
equipment_status_logsplloà (0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h :C
A
memberà$ («2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:j"h
memberplloà (0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h :E
C
sms_logs$· (2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:l"j
sms_logspllo$ (0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h :J
H
warning_logs· (2$	Ò?WWWWWWç?ë?!      ð?8 Bpllo:q"o
warning_logspllo (0:Description...BArial Unicode MSHR$	É?É?É?!      ð?X ` h RArial Unicode MSX` h p ( 0:
pllo