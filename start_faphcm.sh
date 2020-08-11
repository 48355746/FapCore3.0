#cd fapcore
cd /var/lib/jenkins-home/workspace/fapcore30
#stop web container [fapcorehcm]
sudo docker stop fapcorehcm
#remove web container [fapcorehcm]
sudo docker rm -f fapcorehcm
#remove web image [fapcore/hcm]
sudo docker rmi -f fapcore/hcm
#build web image [fapcore/hcm]
sudo docker build --pull -t fapcore/hcm -f product/hcm/Fap.Hcm.Web/Dockerfile .
#docker run container,���ݾ����£�-v ʱ��,-v logs,-v ������˵��������appsetting.json��־·��ΪLogPath��/var/fapcore/logs�������ø���·��Ϊ��/var/fapcore/annex��
sudo docker run --name fapcorehcm -d -p 5000:80 -p 5001:443 -v /etc/localtime:/etc/localtime -v /usr/docker/fapcorehcm/logs:/var/fapcore/logs -v /usr/docker/fapcorehcm/annex:/var/fapcore/annex fapcore/hcm
#rm Tag<none> images
#docker rmi $(docker images | awk '/^<none>/ { print $3 }')

#�Ӹ�Ŀ¼�����������������������滻��������
#docker-compose build
#docker-compose up
#docker-compose -f docker-compose.yml -f docker-compose.override.yml build
#docker-compose up

#webapi docker build
#cd fapcore
#cd /var/lib/jenkins-home/workspace/fapcore30
#stop web container [fapcorehcmapi]
#sudo docker stop fapcorehcmapi
#remove web container [fapcorehcmapi]
#sudo docker rm -f fapcorehcmapi
#remove web image [fapcorehcmapi]
#sudo docker rmi -f fapcore/hcmapi
#build web image [fapcore/hcmapi]
#docker run container,���ݾ����£�-v ʱ��,-v logs,-v ������˵��������appsetting.json��־·��ΪLogPath��/var/fapcore/logs�������ø���·��Ϊ��/var/fapcore/annex��
#sudo docker build --pull -t fapcore/hcmapi -f product/hcm/Fap.Hcm.WebApi/Dockerfile .
#sudo docker run --name fapcorehcmapi -d -p 5002:80 -p 5003:443 -v /etc/localtime:/etc/localtime -v /usr/docker/fapcorehcm/logs:/var/fapcore/logs -v /usr/docker/fapcorehcm/annex:/var/fapcore/annex fapcore/hcmapi
