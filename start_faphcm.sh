#cd fapcore
cd /var/lib/jenkins-home/workspace/fapcore30
#stop web container [fapcorehcm]
sudo docker stop fapcorehcm
#remove web container [fapcorehcm]
sudo docker rm -f fapcorehcm
#remove web image [fapcore/hcm]
sudo docker rmi -f fapcore/hcm 
#build web image [fapcore/hcm]
sudo docker build -t fapcore/hcm .
#docker run container,数据卷如下：-v 时区,-v logs,-v 附件。说明：设置nlog.config日志路径为（/var/fapcore/logs），设置附件路径为（/var/fapcore/annex）
sudo docker run --name fapcorehcm -d -p 5000:80 -p 5001:443 -v /etc/localtime:/etc/localtime -v /usr/docker/fapcorehcm/logs:/var/fapcore/logs -v /usr/docker/fapcorehcm/annex:/var/fapcore/annex fapcore/hcm